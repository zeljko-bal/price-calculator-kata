namespace PriceCalculator

type Product = { 
    Name : string
    UPC : int
    Price : decimal 
}

module PriceCalculator = 

    let private calculateTaxAmount taxRate price = 
        price * decimal taxRate / 100m
    
    let private roundTo2decimals (value: decimal) = 
        System.Math.Round (value, 2)

    let calculatePrice taxRate product = 
        let taxAmount = calculateTaxAmount taxRate >> roundTo2decimals
        product.Price + taxAmount product.Price
