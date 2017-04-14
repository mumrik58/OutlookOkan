﻿using Outlook = Microsoft.Office.Interop.Outlook;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace OutlookAddIn
{
    public partial class ConfirmWindow : Form
    {
        /// <summary>
        /// メール送信の確認画面を表示。
        /// </summary>
        /// <param name="mail">送信するメールに関する情報</param>
        public ConfirmWindow(Outlook._MailItem mail)
        {
            InitializeComponent();

            GetRecipient(mail);
        }

        /// <summary>
        /// 送信先メールアドレスを取得し、画面に表示する。
        /// </summary>
        /// <param name="mail">送信するメールに関する情報</param>
        public void GetRecipient(Outlook._MailItem mail)
        {
            var displayNameAndRecipient = new Dictionary<string, string>();

            foreach (Outlook.Recipient recip in mail.Recipients)
            {
                // Exchangeの連絡先に登録された情報を取得。
                var exchangedUser = recip.AddressEntry.GetExchangeUser();

                // ローカルの連絡先に登録された情報を取得。
                var registeredUser = recip.AddressEntry.GetContact();

                // 登録されたメールアドレスの場合、登録名のみが表示されるため、メールアドレスと共に表示されるよう表示用テキストを生成。
                var nameAndMailAddress = exchangedUser != null
                    ? exchangedUser.Name + @" (" + exchangedUser.PrimarySmtpAddress + @")"
                    : registeredUser != null
                        ? recip.Name
                        : recip.Address;

                displayNameAndRecipient[recip.Name] = nameAndMailAddress;
            }

            // 宛先(To,CC,BCC)に登録された宛先又は登録名を配列に格納。
            var toAdresses = mail.To?.Split(';') ?? new string[] { };
            var ccAdresses = mail.CC?.Split(';') ?? new string[] { };
            var bccAdresses = mail.BCC?.Split(';') ?? new string[] { };

            // 宛先や登録名から、表示用テキスト(メールアドレスや登録名)を各エリアに表示。
            foreach (var i in displayNameAndRecipient)
            {
                if (toAdresses.Any(address => address.Contains(i.Key)))
                    ToAddressList.Items.Add(i.Value);

                if (ccAdresses.Any(address => address.Contains(i.Key)))
                    CcAddressList.Items.Add(i.Value);

                if (bccAdresses.Any(address => address.Contains(i.Key)))
                    BccAddressList.Items.Add(i.Value);
            }
        }

        private void ToAddressList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendButtonSwitch();
        }

        private void CcAddressList_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SendButtonSwitch();
        }

        private void BccAddressList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendButtonSwitch();
        }

        /// <summary>
        /// 全てのチェックボックスにチェックされた場合のみ、送信ボタンを有効とする。
        /// </summary>
        private void SendButtonSwitch()
        {
            if (ToAddressList.CheckedItems.Count == ToAddressList.Items.Count && CcAddressList.CheckedItems.Count == CcAddressList.Items.Count && BccAddressList.CheckedItems.Count == BccAddressList.Items.Count)
            {
                sendButton.Enabled = true;
            }else
            {
                sendButton.Enabled = false;
            }
        }

    }
}