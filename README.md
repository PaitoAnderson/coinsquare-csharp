# coinsquare-csharp
Unofficial Coinsquare.io API for C#

## Usage
Just copy [CoinsquareAPI.cs](CoinsquareAPI.cs) to your .Net project. Quotes must be divided by 100000000

Please be nice to their endpoints and don't abuse them!

## Limitations
 - Login() must be called manually when the session times out
 
## Example Trade

Buying 100 CAD of BTC
```csharp
var tradeQuote = await _coinsquare.GetQuickTrade("CAD", "BTC", 10000);
var trade = await _coinsquare.QuickTrade("CAD", "BTC", 10000, tradeQuote.coin_request);
```

## Hi, Coinsquare!

If you would like this project removed from GitHub please reach out via my email in my GitHub profile and I will take it down immediately. Thanks!

## License
[MIT](LICENSE)
