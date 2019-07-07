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
    member this.``calculatePrice, when given tax rate, calculates correct final price`` 
        (taxRate: int) (expectedFinalPrice: double) =
            let price = PriceCalculator.calculatePrice taxRate [] product
            Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)

    [<DataRow(20, 15, 21.26)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and discount rate, calculates correct final price`` 
        (taxRate: int) (discountRate: int) (expectedFinalPrice: double) =
            let price = PriceCalculator.calculatePrice taxRate [UniversalDiscount {Rate = discountRate}] product
            Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
        
    [<DataRow(20, 15, 7, 12345, 4.05, 4.46, 19.84)>]
    [<DataRow(21, 15, 7, 789, 4.25, 3.04, 21.46)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
            let price = PriceCalculator.calculatePrice taxRate [UniversalDiscount {Rate = unDiscountRate}; UPCDiscount {Rate = upcDiscountRate; UPC = upc}] product
            Assert.AreEqual(decimal taxAmount, price.TaxAmount)
            Assert.AreEqual(decimal discountAmount, price.DiscountAmount)
            Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
