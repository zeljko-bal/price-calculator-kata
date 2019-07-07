
namespace Test

open Microsoft.VisualStudio.TestTools.UnitTesting
open PriceCalculator
open System

[<TestClass>]
type PriceReportGeneratorTests () =

    let product = { 
        Name = "The Little Prince"
        UPC = 12345
        Price = 20.25m
    }

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate, generates a correct report`` () =
        let price = PriceCalculator.calculatePrice 20 0 product
        let report = PriceReportGenerator.generatePriceReport price
        Assert.AreEqual("Tax amount = $4.05" + Environment.NewLine + 
            "Price before = $20.25, price after = $24.30", 
            report)

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate and discount rate, generates a correct report`` () =
        let price = PriceCalculator.calculatePrice 20 15 product
        let report = PriceReportGenerator.generatePriceReport price
        Assert.AreEqual("Tax amount = $4.05" + Environment.NewLine + 
            "Discount amount = $3.04"+ Environment.NewLine + 
            "Price before = $20.25, price after = $21.26", 
            report)