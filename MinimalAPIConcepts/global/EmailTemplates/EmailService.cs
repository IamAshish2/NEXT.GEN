namespace NEXT.GEN.global.EmailTemplates;

public class EmailTemplates
{
   public static string GetVerificationForSignupEmailTemplate(string otp)
{
    return $@"<!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Verify Your Email Address</title>
            <style>
                body {{
                    font-family: sans-serif;
                    line-height: 1.6;
                    color: #333;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #fff;
                    padding: 30px;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
                h1 {{
                    color: #007bff;
                    margin-bottom: 20px;
                    text-align: center;
                }}
                p {{
                    margin-bottom: 15px;
                }}
                .otp-container {{
                    background-color: #e9ecef;
                    padding: 15px;
                    border-radius: 6px;
                    text-align: center;
                    margin-bottom: 20px;
                }}
                .otp {{
                    font-size: 24px;
                    font-weight: bold;
                    color: #28a745;
                    letter-spacing: 10px;
                }}
                .button {{
                    display: inline-block;
                    background-color: #007bff;
                    color: #fff;
                    padding: 10px 20px;
                    text-decoration: none;
                    border-radius: 5px;
                }}
                .button:hover {{
                    background-color: #0056b3;
                }}
                .disclaimer {{
                    font-size: 0.8em;
                    color: #777;
                    margin-top: 20px;
                }}
                .footer {{
                    margin-top: 30px;
                    text-align: center;
                    color: #777;
                    font-size: 0.9em;
                }}
            </style>
        </head>
        <body>
        <div class=""container"">
            <h1>Verify Your Email Address</h1>
            <p>Hello,</p>
            <p>Thank you for signing up or attempting to access a feature that requires email verification. To complete the process, please use the One-Time Password (OTP) provided below:</p>

            <div class=""otp-container"">
                <span class=""otp"">{otp}</span>
            </div>

            <p>Please enter this OTP on the verification page. This OTP is valid for a limited time.</p>

            <p>If you did not request this verification, you can safely ignore this email. No action is required on your part.</p>

            <p>Thank you,</p>
            <p>The NEXT.GEN Team</p>

            <div class=""disclaimer"">
                <p>This is an automatically generated email. Please do not reply to this message.</p>
            </div>

            <div class=""footer"">
                &copy; {DateTime.UtcNow.Year} NEXT.GEN. All rights reserved.
            </div>
        </div>
        </body>
        </html>";
}

    public static string GetForgetPasswordCodeRequestEmailTemplate(string code)
    {
        return $@"
            <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Password Reset Request</title>
                    <style>
                        body {{
                            font-family: sans-serif;
                            line-height: 1.6;
                            color: #333;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #fff;
                            padding: 30px;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #007bff;
                            margin-bottom: 20px;
                            text-align: center;
                        }}
                        p {{
                            margin-bottom: 15px;
                        }}
                        .otp-container {{
                            background-color: #e9ecef;
                            padding: 15px;
                            border-radius: 6px;
                            text-align: center;
                            margin-bottom: 20px;
                        }}
                        .otp {{
                            font-size: 24px;
                            font-weight: bold;
                            color: #28a745;
                            letter-spacing: 10px;
                        }}
                        .button {{
                            display: inline-block;
                            background-color: #007bff;
                            color: #fff;
                            padding: 10px 20px;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .button:hover {{
                            background-color: #0056b3;
                        }}
                        .disclaimer {{
                            font-size: 0.8em;
                            color: #777;
                            margin-top: 20px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            text-align: center;
                            color: #777;
                            font-size: 0.9em;
                        }}
                    </style>
                </head>
                <body>
                <div class=""container"">
                    <h1>Password Reset Request</h1>
                    <p>Hello,</p>
                    <p>We received a request to reset your password for your NEXT.GEN account. To complete the password reset process, please use the following One-Time Password (OTP):</p>
                    <div class=""otp-container"">
                        <span class=""otp"">{code}</span>
                    </div>

                    <p>This OTP is valid for 15 minutes. If you didn't request a password reset, please ignore this email or contact our support team immediately.</p>

                    <p>For security reasons:</p>
                    <ul>
                        <li>Never share this OTP with anyone</li>
                        <li>Our team will never ask for your OTP</li>
                        <li>The OTP will expire after use</li>
                    </ul>

                    <p>If you need any assistance, please contact our <a href=""{{{{SupportLink}}}}"">support team</a>.</p>

                    <div class=""disclaimer"">
                        <p>This is an automatically generated email. Please do not reply to this message.</p>
                    </div>

                    <div class=""footer"">
                        &copy; {DateTime.UtcNow.Year} NEXT.GEN. All rights reserved.<br>
                        <a href=""{{{{CompanyLink}}}}"">Visit our website</a> | <a href=""{{{{PrivacyLink}}}}"">Privacy Policy</a>
                    </div>
                </div>
               </body>
              </html>
        ";
    }
}

     /*
            string emailBody = $@"
                                <!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                    <meta charset=""UTF-8"">
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                    <title>Verify Your Email Address</title>
                                    <style>
                                        body {{
                                            font-family: sans-serif;
                                            line-height: 1.6;
                                            color: #333;
                                            background-color: #f4f4f4;
                                            margin: 0;
                                            padding: 0;
                                        }}
                                        .container {{
                                            max-width: 600px;
                                            margin: 20px auto;
                                            background-color: #fff;
                                            padding: 30px;
                                            border-radius: 8px;
                                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                        }}
                                        h1 {{
                                            color: #007bff;
                                            margin-bottom: 20px;
                                            text-align: center;
                                        }}
                                        p {{
                                            margin-bottom: 15px;
                                        }}
                                        .otp-container {{
                                            background-color: #e9ecef;
                                            padding: 15px;
                                            border-radius: 6px;
                                            text-align: center;
                                            margin-bottom: 20px;
                                        }}
                                        .otp {{
                                            font-size: 24px;
                                            font-weight: bold;
                                            color: #28a745;
                                            letter-spacing: 10px;
                                        }}
                                        .button {{
                                            display: inline-block;
                                            background-color: #007bff;
                                            color: #fff;
                                            padding: 10px 20px;
                                            text-decoration: none;
                                            border-radius: 5px;
                                        }}
                                        .button:hover {{
                                            background-color: #0056b3;
                                        }}
                                        .disclaimer {{
                                            font-size: 0.8em;
                                            color: #777;
                                            margin-top: 20px;
                                        }}
                                        .footer {{
                                            margin-top: 30px;
                                            text-align: center;
                                            color: #777;
                                            font-size: 0.9em;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class=""container"">
                                        <h1>Verify Your Email Address</h1>
                                        <p>Hello,</p>
                                        <p>Thank you for signing up or attempting to access a feature that requires email verification. To complete the process, please use the One-Time Password (OTP) provided below:</p>

                                        <div class=""otp-container"">
                                            <span class=""otp"">{otp}</span>
                                        </div>

                                        <p>Please enter this OTP on the verification page. This OTP is valid for a limited time.</p>

                                        <p>If you did not request this verification, you can safely ignore this email. No action is required on your part.</p>

                                        <p>Thank you,</p>
                                        <p>The NEXT.GEN Team</p>

                                        <div class=""disclaimer"">
                                            <p>This is an automatically generated email. Please do not reply to this message.</p>
                                        </div>

                                        <div class=""footer"">
                                            &copy; [{DateTime.UtcNow.Year}] [NEXT.GEN]. All rights reserved.
                                        </div>
                                    </div>
                                </body>
                                </html>";
                */