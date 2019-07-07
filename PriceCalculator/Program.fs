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
        let discountRate = 15
        
        let price = PriceCalculator.calculatePrice taxRate discountRate product

        printfn "Tax=%d%%, discount=%d%%" taxRate discountRate
        printfn "Tax amount = $%M; Discount amount = $%M" price.TaxAmount price.DiscountAmount
        printfn "Price before = $%M, price after = $%M" price.BaseAmount price.FinalAmount

    [<EntryPoint>]
    let main argv = 
        test
        Console.ReadKey() |> ignore
        0
