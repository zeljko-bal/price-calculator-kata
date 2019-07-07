namespace PriceCalculator

type CalculatedPrice = {
    BaseAmount : decimal
    TaxAmount : decimal
    DiscountAmount : decimal
    FinalAmount : decimal
}

module PriceCalculator = 

    let private calculatePercentage (percentage: int) wholeAmount = 
        wholeAmount * decimal percentage / 100m
        
    let private roundTo2decimals (value: decimal) = 
        System.Math.Round (value, 2)

    let private calculateTaxAmount = calculatePercentage

    let private calculateDiscountAmount = calculatePercentage

    let calculatePrice taxRate discountRate product = 
        let taxAmount = calculateTaxAmount taxRate product.Price |> roundTo2decimals
        let discountAmount = calculateDiscountAmount discountRate product.Price |> roundTo2decimals
        let finalAmount = product.Price + taxAmount - discountAmount
        {
            BaseAmount = product.Price
            TaxAmount = taxAmount
            DiscountAmount = discountAmount
            FinalAmount = finalAmount
        }

