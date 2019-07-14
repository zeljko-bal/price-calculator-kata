namespace PriceCalculator

module PriceCalculation = 
    open PriceDefinition
    open Model

    type CalculatedExpense = {
        Name : string
        Amount : Money
    }

    type CalculatedPrice = {
        BaseAmount : Money
        TaxAmount : Money
        DiscountAmount : Money
        Expenses : CalculatedExpense seq
        FinalAmount : Money
    }

    let zeroOfSameCurrency (money: Money) = money.ZeroOfSameCurrency

    let private calculatePercentage (percentage: int) (wholeAmount: Money) = 
        wholeAmount * decimal percentage / 100m
        
    let private roundTo (decimals: int) (value: Money) = 
        value.round decimals

    let private calculateTaxAmount precision rate = 
        calculatePercentage rate 
        >> roundTo precision

    let private calculateUniversalDiscountAmount (precision: int) (discount: UniversalDiscount) = 
        calculatePercentage discount.Rate
        >> roundTo precision

    let private calculateUPCDiscountAmount (precision: int) (discount: UPCDiscount) ({UPC = productUpc}: Product) = 
        match discount.UPC with
        | discountUpc when discountUpc = productUpc -> calculatePercentage discount.Rate
        | _ -> zeroOfSameCurrency
        >> roundTo precision

    let rec private calculateDiscountAmount precision discount product = 
        match discount with
        | UniversalDiscount discount -> calculateUniversalDiscountAmount precision discount
        | UPCDiscount discount -> calculateUPCDiscountAmount precision discount product
        | AdditiveDiscounts discounts -> calculateAdditiveDiscountsAmount precision discounts product
        | MultiplicativeDiscounts discounts -> calculateMultiplicativeDiscountsAmount precision discounts product
        | NoDiscount -> zeroOfSameCurrency

    and private calculateAdditiveDiscountsAmount precision discounts product basePrice = 
        let combineAdditiveDiscounts currentDiscountAmount discount =
            currentDiscountAmount + calculateDiscountAmount precision discount product basePrice
        
        discounts
        |> Seq.fold combineAdditiveDiscounts basePrice.ZeroOfSameCurrency
        |> roundTo precision

    and private calculateMultiplicativeDiscountsAmount precision discounts product basePrice = 
        let combineMultiplicativeDiscounts currentDiscountAmount discount =
            currentDiscountAmount + calculateDiscountAmount precision discount product (basePrice - currentDiscountAmount)

        discounts
        |> Seq.fold combineMultiplicativeDiscounts basePrice.ZeroOfSameCurrency
        |> roundTo precision

    let private calculateExpenseAmount precision expense product = 
        match expense with
        | PercentageExpense expense -> calculatePercentage expense.Percentage product.Price
        | AbsoluteExpense expense -> expense.Amount
        |> roundTo precision

    let private extractExpenseName expense = 
        match expense with
        | PercentageExpense expense -> expense.Name
        | AbsoluteExpense expense -> expense.Name

    let private calculateExpenses precision expenses product = 
        expenses
        |> Seq.map (fun expense -> { 
            Name = extractExpenseName expense
            Amount = calculateExpenseAmount precision expense product
        })

    let private calculateExpensesAmount precision calculatedExpenses = 
        if Seq.isEmpty calculatedExpenses then None
        else Some (calculatedExpenses
            |> Seq.map (fun expense -> expense.Amount)
            |> Seq.reduce (+)
            |> roundTo precision)

    let private applyDiscountCap cap product discountAmount = 
        let cappedAmount = 
            match cap with
            | Percentage percentage -> calculatePercentage percentage product.Price
            | Absolute amount -> amount
            | Unbound -> discountAmount
        min cappedAmount discountAmount

    let calculatePrice priceDefinition product = 
        let discountAmountBeforeTax = calculateDiscountAmount priceDefinition.Precision priceDefinition.Discounts.BeforeTax product product.Price
        let priceAmountBeforeTax = product.Price - discountAmountBeforeTax
        let taxAmount = calculateTaxAmount priceDefinition.Precision priceDefinition.TaxRate priceAmountBeforeTax
        let discountAmountAfterTax = calculateDiscountAmount priceDefinition.Precision priceDefinition.Discounts.AfterTax product priceAmountBeforeTax
        let discountAmount = discountAmountBeforeTax + discountAmountAfterTax
        let cappedDiscountAmount = applyDiscountCap priceDefinition.Discounts.Cap product discountAmount
        let calculatedExpenses = calculateExpenses priceDefinition.Precision priceDefinition.Expenses product
        let expensesAmount = 
            calculateExpensesAmount priceDefinition.Precision calculatedExpenses 
            |> Option.defaultValue product.Price.ZeroOfSameCurrency
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
