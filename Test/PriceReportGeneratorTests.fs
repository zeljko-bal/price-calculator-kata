﻿
namespace Test

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open PriceCalculator.PriceCalculation
open PriceCalculator.PriceDefinition
open PriceCalculator.PriceReportGenerator
open PriceCalculator.Model

[<TestClass>]
type PriceReportGeneratorTests () =

    let product = { 
        Name = "The Little Prince"
        UPC = 12345
        Price = Money.Of 20.25m "USD"
    }

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate, generates a correct report`` () =
        let price = 
            definePrice
            |> withTax 20
            |> calculatePriceForProduct product
        let report = generatePriceReport price
        Assert.AreEqual("Tax amount = 4.05 USD" + Environment.NewLine + 
            "Price before = 20.25 USD, price after = 24.30 USD", 
            report)

    [<DataRow("RSD")>]
    [<DataRow("USD")>]
    [<DataRow("GBP")>]
    [<DataTestMethod>]
    member this.``generatePriceReport, when given currency, generates a correct report`` (currency: string) =
        let priceAmount = 20.25m
        let product = { 
            Name = "The Little Prince"
            UPC = 12345
            Price = Money.Of priceAmount "USD"
        }
        
        let price = 
            definePrice
            |> withTax 20
            |> calculatePriceForProduct {product with Price = Money.Of priceAmount currency}
        let report = generatePriceReport price
        Assert.AreEqual(
            sprintf "Tax amount = 4.05 %s" currency + Environment.NewLine + 
            sprintf "Price before = 20.25 %s, price after = 24.30 %s" currency currency, 
            report)

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate and discount rate, generates a correct report`` () =
        let price = 
            definePrice
            |> withTax 20
            |> withDiscountsAfterTax (AdditiveDiscounts 
                [UniversalDiscount {Rate = 15}])
            |> calculatePriceForProduct product
        let report = generatePriceReport price
        Assert.AreEqual("Tax amount = 4.05 USD" + Environment.NewLine + 
            "Discount amount = 3.04 USD"+ Environment.NewLine + 
            "Price before = 20.25 USD, price after = 21.26 USD", 
            report)

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate and discount rate and expenses, generates a correct report`` () =
        let price = 
            definePrice
            |> withTax 21
            |> withDiscountsAfterTax (AdditiveDiscounts [
                UniversalDiscount {Rate = 15} 
                UPCDiscount {Rate = 7; UPC = 12345} ])
            |> withExpenses [
                PercentageExpense {Name = "Packaging"; Percentage = 1}
                AbsoluteExpense {Name = "Transport"; Amount = Money.Of 2.2m "USD"} ]
            |> calculatePriceForProduct product
        let report = generatePriceReport price
        Assert.AreEqual("Tax amount = 4.25 USD" + Environment.NewLine + 
            "Discount amount = 4.46 USD"+ Environment.NewLine + 
            "Packaging = 0.20 USD"+ Environment.NewLine + 
            "Transport = 2.2 USD"+ Environment.NewLine + 
            "Price before = 20.25 USD, price after = 22.44 USD", 
            report)