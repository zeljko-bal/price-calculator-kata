﻿namespace PriceCalculator

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
        | AdditiveDiscounts of Discount seq
        | MultiplicativeDiscounts of Discount seq
        | NoDiscount

    type DiscountCap = 
        | Percentage of int
        | Absolute of decimal
        | Unbound

    type Discounts = {
        BeforeTax : Discount
        AfterTax : Discount
        Cap : DiscountCap
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
            BeforeTax = NoDiscount
            AfterTax = NoDiscount
            Cap = Unbound
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

    let withDisountCap cap priceDefinition = 
        { priceDefinition with Discounts = { priceDefinition.Discounts with Cap = cap } }
