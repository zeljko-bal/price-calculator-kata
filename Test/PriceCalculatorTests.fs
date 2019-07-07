namespace Test

open Microsoft.VisualStudio.TestTools.UnitTesting
open PriceCalculator

[<TestClass>]
type PriceCalculatorTests () =

    let product = { 
        Name = "The Little Prince"
        UPC = 12345
        Price = 20.25m
    }

    [<DataRow(20, 24.30)>]
    [<DataRow(21, 24.50)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate, calculates correct final price`` (taxRate: int) (expectedFinalPrice: double) =
        let price = PriceCalculator.calculatePrice taxRate 0 product
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)

    [<DataRow(20, 15, 21.26)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and discount rate, calculates correct final price`` (taxRate: int) (discountRate: int) (expectedFinalPrice: double) =
        let price = PriceCalculator.calculatePrice taxRate discountRate product
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
