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
    member this.``CalculatePrice Calculates Correct FinalPrice`` (taxRate: int) (expectedFinalPrice: double) =
        let finalPrice = PriceCalculator.calculatePrice taxRate product
        Assert.AreEqual(decimal expectedFinalPrice, finalPrice)
