namespace Test

open Microsoft.VisualStudio.TestTools.UnitTesting
open PriceCalculator.PriceCalculation
open PriceCalculator.PriceDefinition
open PriceCalculator.Model

[<TestClass>]
type PriceCalculatorTests () =

    let product = { 
        Name = "The Hitchhiker's Guide to the Galaxy"
        UPC = 12345
        Price = 20.25m
    }

    [<DataRow(20, 24.30)>]
    [<DataRow(21, 24.50)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate, calculates correct final price`` 
        (taxRate: int) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> calculatePriceForProduct product
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)

    [<DataRow(20, 15, 21.26)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and discount rate, calculates correct final price`` 
        (taxRate: int) (discountRate: int) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax [UniversalDiscount {Rate = discountRate}]
            |> calculatePriceForProduct product
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
        
    [<DataRow(20, 15, 7, 12345, 4.05, 4.46, 19.84)>]
    [<DataRow(21, 15, 7, 789, 4.25, 3.04, 21.46)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax [
                UniversalDiscount {Rate = unDiscountRate} 
                UPCDiscount {Rate = upcDiscountRate; UPC = upc} ]
            |> calculatePriceForProduct product
        Assert.AreEqual(decimal taxAmount, price.TaxAmount)
        Assert.AreEqual(decimal discountAmount, price.DiscountAmount)
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
        
    [<DataRow(20, 15, 7, 12345, 3.77, 4.24, 19.78)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount after tax and upc discount before tax, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withDiscountsBeforeTax [UPCDiscount {Rate = upcDiscountRate; UPC = upc}]
            |> withTax taxRate
            |> withDiscountsAfterTax [UniversalDiscount {Rate = unDiscountRate}]
            |> calculatePriceForProduct product
        Assert.AreEqual(decimal taxAmount, price.TaxAmount)
        Assert.AreEqual(decimal discountAmount, price.DiscountAmount)
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
        
    [<DataRow(21, 15, 7, 12345, 4.25, 4.46, 22.44)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount and expenses, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax [
                UniversalDiscount {Rate = unDiscountRate} 
                UPCDiscount {Rate = upcDiscountRate; UPC = upc} ]
            |> withExpenses [
                PercentageExpense {Name = "Packaging"; Percentage = 1}
                AbsoluteExpense {Name = "Transport"; Amount = 2.2m} ]
            |> calculatePriceForProduct product
        Assert.AreEqual(decimal taxAmount, price.TaxAmount)
        Assert.AreEqual(decimal discountAmount, price.DiscountAmount)
        Assert.AreEqual(decimal expectedFinalPrice, price.FinalAmount)
