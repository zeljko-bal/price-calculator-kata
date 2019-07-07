namespace PriceCalculator

open System

module Main = 

    let test = 
        let product = { 
            Name = "The Little Prince"
            UPC = 12345
            Price = 20.25m
        }

        let taxRate = 20
        
        let price = PriceCalculator.calculatePrice taxRate [UniversalDiscount { Rate = 15 }] product

        PriceReportGenerator.generatePriceReport price |> printf "%s"

    [<EntryPoint>]
    let main argv = 
        test
        Console.ReadKey() |> ignore
        0
