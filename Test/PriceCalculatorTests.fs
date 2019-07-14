namespace Test

open Microsoft.VisualStudio.TestTools.UnitTesting
open PriceCalculator.PriceCalculation
open PriceCalculator.PriceDefinition
open PriceCalculator.Model
open TestUtils
open System

[<TestClass>]
type PriceCalculatorTests () =

    let product = { 
        Name = "The Hitchhiker's Guide to the Galaxy"
        UPC = 12345
        Price = Money.Of 20.25m "USD"
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
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)

    [<DataRow(20, 15, 21.26)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and discount rate, calculates correct final price`` 
        (taxRate: int) (discountRate: int) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax (AdditiveDiscounts 
                [UniversalDiscount {Rate = discountRate}])
            |> calculatePriceForProduct product
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)
        
    [<DataRow(20, 15, 7, 12345, 4.05, 4.46, 19.84)>]
    [<DataRow(21, 15, 7, 789, 4.25, 3.04, 21.46)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax (AdditiveDiscounts [
                UniversalDiscount {Rate = unDiscountRate} 
                UPCDiscount {Rate = upcDiscountRate; UPC = upc} ])
            |> calculatePriceForProduct product
        Assert.AreEqual(taxAmount |> toMoney, price.TaxAmount)
        Assert.AreEqual(discountAmount |> toMoney, price.DiscountAmount)
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)
        
    [<DataRow(20, 15, 7, 12345, 3.77, 4.24, 19.78)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount after tax and upc discount before tax, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withDiscountsBeforeTax (AdditiveDiscounts 
                [UPCDiscount {Rate = upcDiscountRate; UPC = upc}])
            |> withTax taxRate
            |> withDiscountsAfterTax (AdditiveDiscounts 
                [UniversalDiscount {Rate = unDiscountRate}])
            |> calculatePriceForProduct product
        Assert.AreEqual(taxAmount |> toMoney, price.TaxAmount)
        Assert.AreEqual(discountAmount |> toMoney, price.DiscountAmount)
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)
        
    [<DataRow(21, 15, 7, 12345, 4.25, 4.46, 22.44)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount and expenses, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax (AdditiveDiscounts [
                UniversalDiscount {Rate = unDiscountRate} 
                UPCDiscount {Rate = upcDiscountRate; UPC = upc} ])
            |> withExpenses [
                PercentageExpense {Name = "Packaging"; Percentage = 1}
                AbsoluteExpense {Name = "Transport"; Amount = Money.Of 2.2m "USD"} ]
            |> calculatePriceForProduct product
        Assert.AreEqual(taxAmount |> toMoney, price.TaxAmount)
        Assert.AreEqual(discountAmount |> toMoney, price.DiscountAmount)
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)
        
    [<DataRow(21, 15, 7, 12345, 4.25, 4.24, 22.66)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount as multiplicative and expenses, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
        
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax (MultiplicativeDiscounts [
                UniversalDiscount {Rate = unDiscountRate} 
                UPCDiscount {Rate = upcDiscountRate; UPC = upc} ])
            |> withExpenses [
                PercentageExpense {Name = "Packaging"; Percentage = 1}
                AbsoluteExpense {Name = "Transport"; Amount = Money.Of 2.2m "USD"} ]
            |> calculatePriceForProduct product
        Assert.AreEqual(taxAmount |> toMoney, price.TaxAmount)
        Assert.AreEqual(discountAmount |> toMoney, price.DiscountAmount)
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)

    [<DataRow(21, 15, 7, 12345, 4.00, 4.25, 4.00, 20.50)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount and absolute cap, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (cap: double) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
    
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax (AdditiveDiscounts [
                UniversalDiscount {Rate = unDiscountRate}
                UPCDiscount {Rate = upcDiscountRate; UPC = upc}])
            |> withDisountCap (Absolute (toMoney cap))
            |> calculatePriceForProduct product
        Assert.AreEqual(taxAmount |> toMoney, price.TaxAmount)
        Assert.AreEqual(discountAmount |> toMoney, price.DiscountAmount)
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)

    [<DataRow(21, 15, 7, 12345, 20, 4.25, 4.05, 20.45)>]
    [<DataRow(21, 15, 7, 12345, 30, 4.25, 4.46, 20.04)>]
    [<DataTestMethod>]
    member this.``calculatePrice, when given tax rate and universal discount and upc discount and percentage cap, calculates correct price`` 
        (taxRate: int) (unDiscountRate: int) (upcDiscountRate: int) (upc: int) (cap: int) (taxAmount: double) (discountAmount: double) (expectedFinalPrice: double) =
    
        let price = 
            definePrice
            |> withTax taxRate
            |> withDiscountsAfterTax (AdditiveDiscounts [
                UniversalDiscount {Rate = unDiscountRate}
                UPCDiscount {Rate = upcDiscountRate; UPC = upc}])
            |> withDisountCap (Percentage cap)
            |> calculatePriceForProduct product
        Assert.AreEqual(taxAmount |> toMoney, price.TaxAmount)
        Assert.AreEqual(discountAmount |> toMoney, price.DiscountAmount)
        Assert.AreEqual(expectedFinalPrice |> toMoney, price.FinalAmount)

    [<TestMethod>]
    member this.``calculatePrice, when given price definition in different currency than the product, throws`` () =
        let calculate1 = 
            definePrice
            |> withDiscountsAfterTax (
                UniversalDiscount {Rate = 5})
            |> withDisountCap (Absolute (5.0 |> toMoneyOfCurrency "RSD"))
            |> calculatePrice

        let calculate2 =
            definePrice
            |> withDiscountsAfterTax (
                UniversalDiscount {Rate = 5})
            |> withExpenses [AbsoluteExpense { Name = ""; Amount = (5.0 |> toMoneyOfCurrency "RSD") }]
            |> calculatePrice

        Assert.ThrowsException<InvalidOperationException>(fun () -> product |> calculate1 |> ignore) |> ignore
        Assert.ThrowsException<InvalidOperationException>(fun () -> product |> calculate2 |> ignore) |> ignore

    [<TestMethod>]
    member this.``calculatePrice, when given higher precision, calculates correct price`` () =
        let price = 
            definePrice
            |> withPrecision 4
            |> withTax 21
            |> withDiscountsAfterTax (MultiplicativeDiscounts [
                UniversalDiscount {Rate = 15} 
                UPCDiscount {Rate = 7; UPC = 12345} ])
            |> withExpenses [PercentageExpense {Name = "Transport"; Percentage = 3}]
            |> calculatePriceForProduct product
        Assert.AreEqual(4.2525 |> toMoney, price.TaxAmount)
        Assert.AreEqual(4.2424 |> toMoney, price.DiscountAmount)
        Assert.AreEqual(20.8676 |> toMoney, price.FinalAmount)