namespace PriceCalculator

module PriceCalculation = 
    open PriceDefinition
    open Model

    type CalculatedExpense = {
        Name : string
        Amount : decimal
    }

    type CalculatedPrice = {
        BaseAmount : decimal
        TaxAmount : decimal
        DiscountAmount : decimal
        Expenses : CalculatedExpense seq
        FinalAmount : decimal
    }

    let private calculatePercentage (percentage: int) wholeAmount = 
        wholeAmount * decimal percentage / 100m
        
    let private roundTo2decimals (value: decimal) = 
        System.Math.Round (value, 2)

    let private calculateTaxAmount rate basePrice = 
        calculatePercentage rate basePrice
        |> roundTo2decimals

    let private calculateUniversalDiscountAmount (discount: UniversalDiscount) (basePrice: decimal) = 
        calculatePercentage discount.Rate basePrice
        |> roundTo2decimals

    let private calculateUPCDiscountAmount (discount: UPCDiscount) (product: Product) (basePrice: decimal) = 
        match discount.UPC with
        | upc when upc = product.UPC -> calculatePercentage discount.Rate basePrice
        | _ -> 0m
        |> roundTo2decimals

    let rec private calculateDiscountAmount discount product basePrice = 
        match discount with
        | UniversalDiscount discount -> calculateUniversalDiscountAmount discount basePrice
        | UPCDiscount discount -> calculateUPCDiscountAmount discount product basePrice
        | AdditiveDiscounts discounts -> calculateAdditiveDiscountsAmount discounts product basePrice
        | MultiplicativeDiscounts discounts -> calculateMultiplicativeDiscountsAmount discounts product basePrice
        | NoDiscount -> 0m

    and private calculateAdditiveDiscountsAmount discounts product basePrice = 
        discounts
        |> Seq.map (fun discount -> calculateDiscountAmount discount product basePrice)
        |> Seq.sum
        |> roundTo2decimals

    and private calculateMultiplicativeDiscountsAmount discounts product basePrice = 
        let combineMultiplicativeDiscounts currentDiscountAmount discount =
            currentDiscountAmount + calculateDiscountAmount discount product (basePrice - currentDiscountAmount)

        discounts
        |> Seq.fold combineMultiplicativeDiscounts 0m
        |> roundTo2decimals

    let calculateExpenseAmount expense product = 
        match expense with
        | PercentageExpense expense -> calculatePercentage expense.Percentage product.Price
        | AbsoluteExpense expense -> expense.Amount
        |> roundTo2decimals

    let extractExpenseName expense = 
        match expense with
        | PercentageExpense expense -> expense.Name
        | AbsoluteExpense expense -> expense.Name

    let calculateExpenses (expenses: Expense seq) product = 
        expenses
        |> Seq.map (fun expense -> { 
            Name = extractExpenseName expense
            Amount = calculateExpenseAmount expense product
        })

    let calculateExpensesAmount calculatedExpenses = 
        calculatedExpenses
        |> Seq.sumBy (fun expense -> expense.Amount)
        |> roundTo2decimals

    let applyDiscountCap cap product discountAmount = 
        let cappedAmount = 
            match cap with
            | Percentage percentage -> calculatePercentage percentage product.Price
            | Absolute amount -> amount
            | Unbound -> discountAmount
        min cappedAmount discountAmount

    let calculatePrice priceDefinition product = 
        let discountAmountBeforeTax = calculateDiscountAmount priceDefinition.Discounts.BeforeTax product product.Price
        let priceAmountBeforeTax = product.Price - discountAmountBeforeTax
        let taxAmount = calculateTaxAmount priceDefinition.TaxRate priceAmountBeforeTax
        let discountAmountAfterTax = calculateDiscountAmount priceDefinition.Discounts.AfterTax product priceAmountBeforeTax
        let discountAmount = discountAmountBeforeTax + discountAmountAfterTax
        let cappedDiscountAmount = applyDiscountCap priceDefinition.Discounts.Cap product discountAmount
        let calculatedExpenses = calculateExpenses priceDefinition.Expenses product
        let expensesAmount = calculateExpensesAmount calculatedExpenses
        let finalAmount = product.Price + taxAmount - cappedDiscountAmount + expensesAmount

        {
            BaseAmount = product.Price
            TaxAmount = taxAmount
            DiscountAmount = cappedDiscountAmount
            Expenses = calculatedExpenses
            FinalAmount = finalAmount
        }

    // A convenience function that takes a product as the first argument, 
    // so that it can be piped into a price definition.
    let calculatePriceForProduct product priceDefinition = 
        calculatePrice priceDefinition product
