
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
        Price = 20.25m
    }

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate, generates a correct report`` () =
        let price = 
            definePrice
            |> withTax 20
            |> calculatePriceForProduct product
        let report = generatePriceReport price
        Assert.AreEqual("Tax amount = $4.05" + Environment.NewLine + 
            "Price before = $20.25, price after = $24.30", 
            report)

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate and discount rate, generates a correct report`` () =
        let price = 
            definePrice
            |> withTax 20
            |> withDiscountsAfterTax [UniversalDiscount {Rate = 15}]
            |> calculatePriceForProduct product
        let report = generatePriceReport price
        Assert.AreEqual("Tax amount = $4.05" + Environment.NewLine + 
            "Discount amount = $3.04"+ Environment.NewLine + 
            "Price before = $20.25, price after = $21.26", 
            report)

    [<TestMethod>]
    member this.``generatePriceReport, when given tax rate and discount rate and expenses, generates a correct report`` () =
        let price = 
            definePrice
            |> withTax 21
            |> withDiscountsAfterTax [
                UniversalDiscount {Rate = 15} 
                UPCDiscount {Rate = 7; UPC = 12345} ]
            |> withExpenses [
                PercentageExpense {Name = "Packaging"; Percentage = 1}
                AbsoluteExpense {Name = "Transport"; Amount = 2.2m} ]
            |> calculatePriceForProduct product
        let report = generatePriceReport price
        Assert.AreEqual("Tax amount = $4.25" + Environment.NewLine + 
            "Discount amount = $4.46"+ Environment.NewLine + 
            "Packaging = $0.20"+ Environment.NewLine + 
            "Transport = $2.2"+ Environment.NewLine + 
            "Price before = $20.25, price after = $22.44", 
            report)