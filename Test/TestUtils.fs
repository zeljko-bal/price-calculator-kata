namespace Test

open PriceCalculator.Model

module TestUtils =

    let toMoneyOfCurrency currency (amount: double) = 
        Money.Of (decimal amount) currency

    let toMoney amount = 
        toMoneyOfCurrency "USD" amount 
