﻿namespace PriceCalculator

module PriceCalculation = 
    open PriceDefinition
    open Model

    type CalculatedPrice = {
        BaseAmount : decimal
        TaxAmount : decimal
        DiscountAmount : decimal
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

    let private calculateDiscountAmount discount product basePrice = 
        match discount with
            | UniversalDiscount discount -> calculateUniversalDiscountAmount discount basePrice
            | UPCDiscount discount -> calculateUPCDiscountAmount discount product basePrice
    
    let private calculateDiscountsAmount discounts product basePrice = 
        discounts
        |> List.map (fun discount -> calculateDiscountAmount discount product basePrice)
        |> List.sum
        |> roundTo2decimals

    let calculatePrice priceDefinition product = 
        let discountAmountBeforeTax = calculateDiscountsAmount priceDefinition.Discounts.BeforeTax product product.Price
        let priceAmountBeforeTax = product.Price - discountAmountBeforeTax
        let taxAmount = calculateTaxAmount priceDefinition.TaxRate priceAmountBeforeTax
        let discountAmountAfterTax = calculateDiscountsAmount priceDefinition.Discounts.AfterTax product priceAmountBeforeTax
        let discountAmount = discountAmountBeforeTax + discountAmountAfterTax
        let finalAmount = product.Price + taxAmount - discountAmount

        {
            BaseAmount = product.Price
            TaxAmount = taxAmount
            DiscountAmount = discountAmount
            FinalAmount = finalAmount
        }

    // A convenience function that takes a product as the first argument, 
    // so that it can be piped into a price definition.
    let calculatePriceForProduct product priceDefinition = 
        calculatePrice priceDefinition product
