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
            Price = 20.25m
        }
             
        let price = 
            definePrice
            |> withDiscountsBeforeTax [UPCDiscount {Rate = 7; UPC = 12345}]
            |> withTax 20
            |> withDiscountsAfterTax [UniversalDiscount {Rate = 15}]
            |> calculatePriceForProduct product

        PriceReportGenerator.generatePriceReport price |> printf "%s"

    [<EntryPoint>]
    let main argv = 
        test
        Console.ReadKey() |> ignore
        0
