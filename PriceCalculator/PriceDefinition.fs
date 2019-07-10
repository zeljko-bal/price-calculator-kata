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
        BeforeTax : Discount list
        AfterTax : Discount list
    }

    type PriceDefinition = {
        TaxRate : int
        Discounts : Discounts
    }

    let definePrice = { TaxRate = 0; Discounts = { BeforeTax = []; AfterTax = [] } }

    let withTax taxRate priceDefinition = 
        { priceDefinition with TaxRate = taxRate }

    let withDiscountsBeforeTax discounts priceDefinition = 
        { priceDefinition with Discounts = { priceDefinition.Discounts with BeforeTax = discounts } }

    let withDiscountsAfterTax discounts priceDefinition = 
        { priceDefinition with Discounts = { priceDefinition.Discounts with AfterTax = discounts } }
