namespace PriceCalculator

module Model = 

    //let private 

    [<CustomComparison>]
    [<CustomEquality>]
    type Money = private { 
        Amount: decimal
        Currency: string 
    }
    with
        member this.round (precesion: int) = 
            { this with Amount = System.Math.Round (this.Amount, precesion) }
        member this.OfSameCurrency amount = 
            { this with Amount = amount }
        member this.ZeroOfSameCurrency = 
            this.OfSameCurrency 0m
        static member ( * ) (lhs: Money, rhs: decimal) = 
            { lhs with Amount = lhs.Amount * rhs }
        static member ( / ) (lhs: Money, rhs: decimal) = 
            { lhs with Amount = lhs.Amount / rhs }
        static member ( + ) (lhs: Money, rhs: Money) =
            if lhs.Currency <> rhs.Currency then Money.throwCurrencyMismatch
            { lhs with Amount = lhs.Amount + rhs.Amount }
        static member ( - ) (lhs: Money, rhs: Money) =
            if lhs.Currency <> rhs.Currency then Money.throwCurrencyMismatch
            { lhs with Amount = lhs.Amount - rhs.Amount }
        override money.ToString() = sprintf "%M %s" money.Amount money.Currency
        static member Of amount currency =
            { Amount = amount; Currency = currency }
        interface System.IComparable<Money> with
            member this.CompareTo { Amount = amount; Currency = currency } =
                if this.Currency <> currency then Money.throwCurrencyMismatch
                this.Amount.CompareTo amount
        interface System.IComparable with
            member this.CompareTo obj =
                match obj with
                | null -> 1
                | :? Money as other -> (this :> System.IComparable<_>).CompareTo other
                | _ -> invalidArg "obj" "not comparable to Money"
        interface System.IEquatable<Money> with
            member this.Equals { Amount = amount; Currency = currency } =
                this.Amount = amount && this.Currency = currency
        override this.Equals obj =
            match obj with
            | :? Money as other -> (this :> System.IEquatable<_>).Equals other
            | _ -> false
        override this.GetHashCode () =
            hash (this.Amount, this.Currency)
        static member private throwCurrencyMismatch = invalidOp "Currency mismatch"

    type Product = { 
        Name : string
        UPC : int
        Price : Money 
    }
