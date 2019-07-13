namespace PriceCalculator

open System
open PriceDefinition
open PriceCalculation
open Model

module Main = 

    let test = 
        let product = { 
            Name = "The Little Prince"
            UPC = 12345
            Price = Money.Of 20.25m "$"
        }
        
        let price = 
            definePrice
            |> withDiscountsBeforeTax (AdditiveDiscounts 
                [UPCDiscount {Rate = 7; UPC = 12345}])
            |> withTax 20
            |> withDiscountsAfterTax (AdditiveDiscounts 
                [UniversalDiscount {Rate = 15}])
            |> calculatePriceForProduct product

        PriceReportGenerator.generatePriceReport price |> printf "%s"

    [<EntryPoint>]
    let main argv = 
        test
        Console.ReadKey() |> ignore
        0
