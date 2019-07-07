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
        
        let finalPrice = PriceCalculator.calculatePrice taxRate product

        sprintf "Product price reported as $%M before tax and $%M after %d%% tax." 
            product.Price finalPrice taxRate

    [<EntryPoint>]
    let main argv = 
        printfn "%s" test
        Console.ReadKey() |> ignore
        0
