namespace PriceCalculator

module PriceDefinition = 

    type UniversalDiscount = {
        Rate : int
    }

    type UPCDiscount = {
        Rate : int
        UPC : int
    }

    type Discount = 
        | UniversalDiscount of UniversalDiscount
        | UPCDiscount of UPCDiscount

    type Discounts = {
        BeforeTax : Discount seq
        AfterTax : Discount seq
    }

    type AbsoluteExpense = {
        Name : string
        Amount : decimal
    }

    type PercentageExpense = {
        Name : string
        Percentage : int
    }

    type Expense = 
        | AbsoluteExpense of AbsoluteExpense
        | PercentageExpense of PercentageExpense

    type PriceDefinition = {
        TaxRate : int
        Discounts : Discounts
        Expenses : Expense seq
    }

    let definePrice = { 
        TaxRate = 0
        Discounts = { 
            BeforeTax = []
            AfterTax = [] 
        }
        Expenses = []
    }

    let withTax taxRate priceDefinition = 
        { priceDefinition with TaxRate = taxRate }

    let withDiscountsBeforeTax discounts priceDefinition = 
        { priceDefinition with Discounts = { priceDefinition.Discounts with BeforeTax = discounts } }

    let withDiscountsAfterTax discounts priceDefinition = 
        { priceDefinition with Discounts = { priceDefinition.Discounts with AfterTax = discounts } }

    let withExpenses expenses priceDefinition = 
        { priceDefinition with Expenses = expenses }
