namespace PriceCalculator

type CalculatedPrice = {
    BaseAmount : decimal
    TaxAmount : decimal
    DiscountAmount : decimal
    FinalAmount : decimal
}

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

module PriceCalculator = 

    let private calculatePercentage (percentage: int) wholeAmount = 
        wholeAmount * decimal percentage / 100m
        
    let private roundTo2decimals (value: decimal) = 
        System.Math.Round (value, 2)

    let private calculateTaxAmount = calculatePercentage

    let private calculateUniversalDiscountAmount (discount: UniversalDiscount) (product: Product) = 
        calculatePercentage discount.Rate product.Price

    let private calculateUPCDiscountAmount (discount: UPCDiscount) (product: Product) = 
        match discount.UPC with
        | upc when upc = product.UPC -> calculatePercentage discount.Rate product.Price
        | _ -> 0m

    let private calculateDiscountAmount discount = 
        match discount with
            | UniversalDiscount universalDiscount -> calculateUniversalDiscountAmount universalDiscount
            | UPCDiscount upcDiscount -> 
                calculateUPCDiscountAmount upcDiscount
    
    let calculatePrice taxRate discounts product = 
        let taxAmount = calculateTaxAmount taxRate product.Price |> roundTo2decimals
        let discountAmount = 
            discounts
            |> List.map (fun discount -> calculateDiscountAmount discount product)
            |> List.map roundTo2decimals
            |> List.sum
            |> roundTo2decimals
        let finalAmount = product.Price + taxAmount - discountAmount

        {
            BaseAmount = product.Price
            TaxAmount = taxAmount
            DiscountAmount = discountAmount
            FinalAmount = finalAmount
        }
