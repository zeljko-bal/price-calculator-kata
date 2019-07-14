namespace PriceCalculator

open System
open PriceDefinition
open PriceCalculation
open Model

module Main = 

    let test = 
        let product = { 
            Name = "The Hitchhiker's Guide to the Galaxy"
            UPC = 12345
            Price = Money.Of 20.25m "$"
        }
        
        let price = 
            definePrice
            |> withDiscountsBeforeTax (UPCDiscount {Rate = 7; UPC = 12345})
            |> withTax 20
            |> withDiscountsAfterTax (UniversalDiscount {Rate = 15})
            |> calculatePriceForProduct product

        let jsonPrice = 
            Configuration.fromJson """
                {
                    "Discounts": {
                        "BeforeTax": {
                            "Type": "UPCDiscount",
                            "Rate": 7,
                            "UPC": 12345
                        },
                        "AfterTax": {
                            "Type": "UniversalDiscount",
                            "Rate": 15
                        }
                    },
                    "TaxRate": 20
                }
            """
            |> calculatePriceForProduct product
        
        printfn "Report:"
        PriceReportGenerator.generatePriceReport 2 price |> printfn "%s"
        printfn "------------"
        printfn "Json Report:"
        PriceReportGenerator.generatePriceReport 2 jsonPrice |> printfn "%s"

    [<EntryPoint>]
    let main _ = 
        test |> ignore
        Console.ReadKey() |> ignore
        0
