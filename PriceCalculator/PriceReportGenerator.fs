namespace PriceCalculator

open System
open PriceCalculation
open Model

module PriceReportGenerator = 
    let private filterSome = 
        Seq.choose id

    let private someWhenGreaterThan0 (amount: Money) someValue = 
        if amount > amount.ZeroOfSameCurrency then Some someValue else None

    let printMoney precision (money: Money) = 
        (money.round precision).ToString()

    let generatePriceReport precision price = 
        let printMoney = printMoney precision
        [
            [
                sprintf "Tax amount = %s" (printMoney price.TaxAmount) |> someWhenGreaterThan0 price.TaxAmount
                sprintf "Discount amount = %s" (printMoney price.DiscountAmount) |> someWhenGreaterThan0 price.DiscountAmount
            ]
            price.Expenses
                |> Seq.map (fun expense -> sprintf "%s = %s" expense.Name (printMoney expense.Amount) |> Some)
                |> Seq.toList
            [
                sprintf "Price before = %s, price after = %s" (printMoney price.BaseAmount) (printMoney price.FinalAmount) |> Some
            ]
        ]
        |> Seq.concat
        |> filterSome
        |> String.concat Environment.NewLine