// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Bogus;
using EncorePosApiFaker.Dto;

Console.WriteLine("Starting application!");


var billTypes = new[] { BillPaymentType.Bill, BillPaymentType.GiftCard };
var stores = Enumerable.Range(1, 9);
var poses = Enumerable.Range(6, 20);
var users = Enumerable.Range(1, 20);
var payments = Enumerable.Range(1, 20);
var semaphore = new SemaphoreSlim(10);

var docId = 1010200;
var fee = 0m;
var invoice = 0m;

var fake = new Faker<BillPayment>()
    .RuleFor(o => o.PosDocumentId, f => docId++)
    .RuleFor(o => o.Type, f => f.PickRandom(billTypes))
    .RuleFor(o => o.DocumentNo, f => f.Commerce.Ean13())
    .RuleFor(o => o.Date, f => f.Date.Between(DateTime.Today.AddMonths(-1), DateTime.Today))
    .RuleFor(o => o.CompanyName, f => f.Company.CompanyName())
    .RuleFor(o => o.IsValid, true)
    .RuleFor(o => o.StoreId, f => f.PickRandom(stores))
    .RuleFor(o => o.PosId, f => f.PickRandom(poses))
    .RuleFor(o => o.UserId, f => f.PickRandom(users))
    .RuleFor(o => o.PaymentId, f => f.PickRandom(payments))
    .RuleFor(o => o.FeeAmount, f => fee = f.Finance.Amount(1m, 5m))
    .RuleFor(o => o.InvoiceAmount, f => invoice = f.Finance.Amount(45m, 245m))
    .RuleFor(o => o.Details, f => new BillPayingDetails
    {
        Description = f.Company.CompanyName(),
        CompanyName = f.Company.CompanyName(),
        FeeAmount = fee,
        InvoiceAmount = invoice,
        PaymentDate = f.Date.Past(),
        InvoiceDate = f.Date.Past(),
        InvoiceNo = f.Finance.Account(),
        SubscriberName = f.Name.FullName(),
        SubscriberNumber = f.Phone.PhoneNumber()
    });
 
var records = fake.Generate(100);

const string baseUrl = "base_url";
using var client = new HttpClient();
using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(baseUrl), "/token_url"));

var tokenBody = new
{
    a = "a",
    b = "b",
    c =  "c"
};

request.Content = new StringContent(JsonSerializer.Serialize(tokenBody), Encoding.UTF8, MediaTypeNames.Application.Json);

var response = await client.SendAsync(request);
var body = await response.Content.ReadFromJsonAsync<TokenDto>();
var token = body!.Token;

foreach (var item in records)
{
    // await semaphore.WaitAsync();

    using var req = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(baseUrl), "/transfer_url"));
    req.Headers.Add("Authorization", $"Bearer {token}");
    req.Content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, MediaTypeNames.Application.Json);

    var res = await client.SendAsync(req);
    var bod = await res.Content.ReadFromJsonAsync<ApiResponseDto>();
    Console.WriteLine(bod);
    if (bod is { FaultyParameters.Count: > 0 })
        Console.WriteLine(string.Join(",", bod.FaultyParameters!.ToArray()));

    // semaphore.Release();
}

//
// Task.WaitAll(CreateCallls().ToArray());
//
// IEnumerable<Task> CreateCallls()
// {
//     foreach (var item in records)
//     {
//         yield return CallApi(item);
//     }
// }
//
// async Task CallApi(BillPayment item)
// {
//     await semaphore.WaitAsync();
//     
//     using var req = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(baseUrl), "/transfer_url"));
//     req.Headers.Add("Authorization", $"Bearer {token}");
//     req.Content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, MediaTypeNames.Application.Json);
//
//     var res = await client!.SendAsync(req);
//     var bod = await res.Content.ReadFromJsonAsync<ApiResponseDto>();
//     Console.WriteLine(bod);
//     if (bod is { FaultyParameters.Count: > 0 })
//         Console.WriteLine(string.Join(",", bod.FaultyParameters!.ToArray()));
//
//     await Task.Delay(200);
//     semaphore.Release();
// }