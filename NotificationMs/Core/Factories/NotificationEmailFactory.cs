using Core.Dtos;
using Core.Enums;
using Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factories
{
    public static class NotificationEmailFactory
    {
        private static string BuildEmailByTemplate(string subject, string body)
        {
            string emailMessage = $@"
                <!DOCTYPE html>
                <html lang=""pt-br"">
                <head>
                    <meta charset=""UTF-8"">
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f9f9f9;
                            padding: 20px;
                            color: #333;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            text-align: center;
                            color: #4CAF50;
                        }}
                        .content {{
                            margin-top: 20px;
                            font-size: 16px;
                            line-height: 1.6;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 13px;
                            color: #888;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <h2 class=""header"">{subject}</h2>
                        <div class=""content"">
                            {body}
                        </div>
                        <div class=""footer"">
                            © {DateTime.Now.Year} - Hackaton FIAP - Software Architecture. Todos os direitos reservados.
                        </div>
                    </div>
                </body>
                </html>";

            return emailMessage;
        }

        public static EmailRequestDto GetVideoProcessedNotificationEmailRequestDto(VideoProcessedEvent videoProcessedEvent)
        {
            string subject = "Vídeo processado com sucesso!";

            string body = $@"<p>Olá,</p>
                            <p>Seu zip com os frames do vídeo ""{videoProcessedEvent.OriginalVideoName}"" está prontinho! Lembre-se, o link funciona por apenas uma hora!</p>
                            <p>Segue abaixo o link para o zip do seu arquivo:</p>
                            <p><a href=""{videoProcessedEvent.ProcessedVideoUrl}"" target=""_blank"">Acessar vídeo processado</a></p>
                            <p>Obrigado pela sua preferência. Esperamos processar ainda mais vídeos para você! 😊</p>";

            string emailMessage = BuildEmailByTemplate(subject, body);

            return new EmailRequestDto
            {
                ToEmail = videoProcessedEvent.UserEmail,
                Subject = subject,
                Body = emailMessage
            };
        }

        public static EmailRequestDto GetVideoProcessingErrorNotificationEmailRequestDto(VideoProcessedEvent videoProcessedEvent)
        {
            string subject = string.Empty;

            string body = $@"<p>Olá,</p>";

            if (videoProcessedEvent.StatusVideoEnum == StatusVideoEnum.NotFound)
            {
                subject = "Vídeo não encontrado!";
                body += $@"<p>Não foi possível processar o seu vídeo!</p>
                           <p>Houve algum problema, e não conseguimos encontrar o seu vídeo ""{videoProcessedEvent.OriginalVideoName}"".</p>
                           <p>Você pode tentar fazer o upload do vídeo novamente!</p>
                           <p>Obrigado pela sua preferência. Esperamos processar ainda mais vídeos para você! 😊</p>";
            }
            else if (videoProcessedEvent.StatusVideoEnum == StatusVideoEnum.Error)
            {
                subject = "Estamos tentando processar o seu vídeo!";
                body += $@"<p>Houve algum problema no processamento do seu vídeo ""{videoProcessedEvent.OriginalVideoName}"".</p>
                           <p>Mas não se preocupe, vamos continuar tentando processá-lo!</p>
                           <p>Obrigado pela sua preferência. Esperamos processar ainda mais vídeos para você! 😊</p>";
            }
            else if (videoProcessedEvent.StatusVideoEnum == StatusVideoEnum.ErrorAttemptsExceeded)
            {
                subject = "Houve um erro no processamento do seu vídeo!";
                body += $@"<p>Não foi possível processar o seu vídeo!</p>
                           <p>Houve algum problema, e infelizmente não conseguimos processar o seu vídeo ""{videoProcessedEvent.OriginalVideoName}"" após várias tentativas.</p>
                           <p>Você pode tentar fazer o upload do vídeo novamente!</p>
                           <p>Obrigado pela sua preferência. Esperamos processar ainda mais vídeos para você! 😊</p>";
            }
            else
            {
                subject = "Houve um erro no processamento do seu vídeo!";
                body += $@"<p>Não foi possível processar o seu vídeo!</p>
                           <p>Houve algum problema, e infelizmente não conseguimos processar o seu vídeo ""{videoProcessedEvent.OriginalVideoName}"".</p>
                           <p>Você pode tentar fazer o upload do vídeo novamente!</p>
                           <p>Obrigado pela sua preferência. Esperamos processar ainda mais vídeos para você! 😊</p>";
            }


            string emailMessage = BuildEmailByTemplate(subject, body);

            return new EmailRequestDto
            {
                ToEmail = videoProcessedEvent.UserEmail,
                Subject = subject,
                Body = emailMessage
            };
        }
    }
}
