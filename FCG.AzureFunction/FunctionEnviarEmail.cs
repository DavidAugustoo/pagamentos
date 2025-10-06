using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FCG.AzureFunction;

public class PagamentoEmailRequest
{
    public int PagamentoId { get; set; }
    public decimal TotalPagamento { get; set; }
    public string FormaPagamento { get; set; }
    public string Nome { get; set; }
    public string NomeJogo { get; set; }
    public string Email { get; set; }
}

public class PagamentoEmailResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
}

public class FunctionEnviarEmail
{
    private readonly ILogger<FunctionEnviarEmail> _logger;
    public FunctionEnviarEmail(ILogger<FunctionEnviarEmail> logger) => _logger = logger;

    [Function("FunctionEnviarEmail")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        try
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var fromAddr = Environment.GetEnvironmentVariable("SENDGRID_FROM");
            var toFallback = Environment.GetEnvironmentVariable("SENDGRID_TO");

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromAddr))
                return new ObjectResult(new PagamentoEmailResponse
                {
                    Sucesso = false,
                    Mensagem = "Faltam SENDGRID_API_KEY e/ou SENDGRID_FROM nas App Settings."
                })
                { StatusCode = StatusCodes.Status500InternalServerError };

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var pagamento = JsonSerializer.Deserialize<PagamentoEmailRequest>(body, opts);

            if (pagamento is null)
                return new BadRequestObjectResult(new PagamentoEmailResponse { Sucesso = false, Mensagem = "Body inválido." });

            var toAddr = string.IsNullOrWhiteSpace(pagamento.Email) ? toFallback : pagamento.Email;
            
            if (string.IsNullOrWhiteSpace(toAddr))
                return new BadRequestObjectResult(new PagamentoEmailResponse
                {
                    Sucesso = false,
                    Mensagem = "Informe 'EmailDestino' no body ou configure SENDGRID_TO nas App Settings."
                });

            var html = @$"
                <!doctype html>
                <html lang='pt-BR'>
                <head>
                  <meta charset='utf-8'>
                  <meta name='viewport' content='width=device-width,initial-scale=1'>
                  <title>Pagamento Realizado</title>
                  <style>
                    /* estilos básicos (seguros p/ e-mail) */
                    body {{
                      margin: 0; padding: 0; background: #f4f6f8; color: #0f172a;
                      font-family: -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
                    }}
                    .wrapper {{ width: 100%; background: #f4f6f8; padding: 24px; }}
                    .container {{
                      max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 14px;
                      box-shadow: 0 8px 24px rgba(0,0,0,.06); overflow: hidden; border: 1px solid #eef2f7;
                    }}
                    .header {{
                      background: linear-gradient(135deg, #12b981, #059669);
                      color: #ffffff; padding: 24px 28px; text-align: center;
                    }}
                    .header h1 {{ margin: 0; font-size: 22px; letter-spacing: .3px; }}
                    .badge {{
                      display: inline-block; margin-top: 10px; padding: 6px 12px; border-radius: 999px;
                      background: rgba(255,255,255,.18); color: #fff; font-size: 12px; font-weight: 600;
                    }}
                    .content {{ padding: 28px; }}
                    .lead {{ margin: 0 0 18px 0; font-size: 16px; line-height: 1.5; color: #334155; }}
                    .card {{
                      border: 1px solid #e5e7eb; border-radius: 12px; padding: 18px; background: #fafafa;
                    }}
                    .table {{
                      width: 100%; border-collapse: separate; border-spacing: 0 10px; font-size: 14px;
                    }}
                    .row {{
                      background: #fff; border: 1px solid #e5e7eb; border-radius: 10px;
                    }}
                    .cell-label {{ width: 45%; padding: 12px 14px; color: #64748b; font-weight: 600; }}
                    .cell-value {{ width: 55%; padding: 12px 14px; color: #0f172a; }}
                    .footer {{
                      text-align: center; color: #94a3b8; font-size: 12px; padding: 18px 12px 6px 12px;
                    }}
                    @media (prefers-color-scheme: dark) {{
                      body {{ background: #0b1220; color: #e5e7eb; }}
                      .container {{ background: #0f172a; border-color: #1f2a44; }}
                      .content .lead {{ color: #cbd5e1; }}
                      .card {{ background: #111827; border-color: #24314d; }}
                      .row {{ background: #0f172a; border-color: #24314d; }}
                      .cell-label {{ color: #93a4c2; }}
                      .cell-value {{ color: #e5e7eb; }}
                      .footer {{ color: #93a4c2; }}
                    }}
                  </style>
                </head>
                <body>
                  <div class='wrapper'>
                    <div class='container'>
                      <div class='header'>
                        <h1>Pagamento Realizado</h1>
                        <span class='badge'>Transação #{pagamento.PagamentoId}</span>
                      </div>

                      <div class='content'>
                        <p class='lead'>
                          Olá, <strong>{pagamento.Nome}</strong>! Recebemos seu pagamento com sucesso.
                          Abaixo seguem os detalhes da transação.
                        </p>

                        <div class='card'>
                          <table class='table' role='presentation' cellpadding='0' cellspacing='0'>
                            <tr class='row'>
                              <td class='cell-label'>Transação</td>
                              <td class='cell-value'>#{pagamento.PagamentoId}</td>
                            </tr>
                            <tr class='row'>
                              <td class='cell-label'>Nome do Usuário</td>
                              <td class='cell-value'>{pagamento.Nome}</td>
                            </tr>
                            <tr class='row'>
                              <td class='cell-label'>E-mail do Usuário</td>
                              <td class='cell-value'>{pagamento.Email}</td>
                            </tr>
                            <tr class='row'>
                              <td class='cell-label'>Jogo</td>
                              <td class='cell-value'>{pagamento.NomeJogo}</td>
                            </tr>
                            <tr class='row'>
                              <td class='cell-label'>Forma de Pagamento</td>
                              <td class='cell-value'>{pagamento.FormaPagamento}</td>
                            </tr>
                            <tr class='row'>
                              <td class='cell-label'>Valor</td>
                              <td class='cell-value'><strong>{pagamento.TotalPagamento:C}</strong></td>
                            </tr>
                          </table>
                        </div>

                        <p class='lead' style='margin-top:18px'>
                          Fiap Cloud Games
                        </p>
                      </div>

                      <div class='footer'>
                        Este é um e-mail automático, não responda. &middot; {DateTime.Now:dd/MM/yyyy HH:mm}
                      </div>
                    </div>
                  </div>
                </body>
                </html>";

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromAddr, "Pagamentos FCG");
            var to = new EmailAddress(toAddr);

            var msg = MailHelper.CreateSingleEmail(from, to, "Confirmação de Pagamento",
                                                   plainTextContent: null, htmlContent: html);

            var sg = await client.SendEmailAsync(msg);

            if (!sg.IsSuccessStatusCode)
            {
                var err = await sg.Body.ReadAsStringAsync();
                _logger.LogError("Erro SendGrid: {Err}", err);
                return new ObjectResult(new PagamentoEmailResponse { Sucesso = false, Mensagem = "Erro ao enviar e-mail: " + err })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new OkObjectResult(new PagamentoEmailResponse { Sucesso = true, Mensagem = "E-mail enviado com sucesso!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail.");
            return new ObjectResult(new PagamentoEmailResponse { Sucesso = false, Mensagem = "Erro ao enviar e-mail: " + ex.Message })
            { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
