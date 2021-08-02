using System;
using System.Collections.Generic;
using System.Text;
using System;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.CommonClasses;
using YPS.Parts2y.Parts2y_Views;

namespace YPS
{
    public class PickerView : View
    {
        public event EventHandler StartScanningRequested;
        public event EventHandler PauseScanningRequested;
        public event EventHandler StopScanningRequested;

        public IScannerDelegate Delegate { get; set; }
        public ScanerSettings Settings { get; set; }

        public static string GetAppKey()
        {
            //return "AY8+nTVOKonoFAi+8CRf9zE00QOpMvmJejGLQpdg7529SntBg3pNgHxtH7vaSEEwECgL8n4Z/qoJXr+kgyqQVw9tVk/RbjaZjDvTvnImbgK9U+cr62hok6VUHkELJoCM5gi2M6ImghM0D9SyWwXKCVx2BYL/5Ki+adHzhHfNP3ZcXQQQ0NznyfNY8md//77xAvwWkSQTPmpvOhcvMJqpXY5TPqPRqiHYUaHJiteGZCmcNsAozWsCabHotiJgsuAFAWftm3IafzUWrN4yaamOuIDyafdAfcaFSp4JwAevtCjnIaJOb3OzyedqdLGlYRrq4MYZmDSIHdulBkrgTDLf2ULoF+6H3NSYggCNX21V9SW5+LqclPzf+SJAymu18ZOkKVzCBeVYhUstCio1H3gJ6jlRjbwOTQ4xnJDV8xoYmFyX0u+KOOVtIYnXiLVHo6Um33QRf8sKKnsu1XvP7wttrG98uGjsF1264pCbDqQs/eV0eldk8EDhMFPaWqN39ajlAAYqmLJOJ0f0PatDZRGV4G6Y5k0wUtNZpHqN/RXhQrQh2lZPLncfk4f6gfds4x+I1ogXmD0+X8S6Gcq2kCFaMXp7jE9JW/BXkZdeSzhws2fGoViE/AM5OnmCvtAjLcdOIVeKUIAqJVtmHg3FqDYTDqiBxHT6ybM5QWADNjMvCqKSpSvWz0PfhS5DXGNL9ST1DFy0wcdoDLUY0druNzijZ17LVmeML9diaTd/tsF+TeLoDviz+ewJfW46OoAsxjOmwTb+doDsoeANwHrolvYC2SmVmRRciuDTxAUqpRKnrIwtvPI5w3Kp2IsSJIOskw==";

            //return "Adqw9B4fPhYNP21WCgk1l7wXPC9aOxpmjzxOZ3lZkQaLQE26EFe + 4oApE0xqRoC + X3J0b2BDk / +Ia8XYK0ANeYZMQtiNaJptPRWC / C1l4aagQcqIBEyKNwxK / Q3yEgINCgqqppIDs5TGLWZRkGao + S7LGESriawbHeK7nopO / aBnjOrpi6P3k5Uxb8vA + d0ute4vf6t8Tr + 3sC0hJV9UuWZNHWw76ySXRyp59l / l0utyCWvQXyXqh2u1hS1vFI2Qr0Yv6Jjk38YUh5dt23FRT2R0qxEOGdcxSFq98V8ReJV / Pxf8v3yy / SAXhtPm6cbIzSni + upB / MlR8wIXGKs12S65eyzEWcMxh7vih98UKvEEnrd2szBFYuX7li0KR0Xe8EKjQR2SsXzJEepGHy4felyn8VlPI + ijL406rVCle / C2OliW49v6etcsAs4Kbq0XGGR9LG9eepUfi1htAlGmCYsQeHZth7DslZMN5EOBemjhijWyIApMfDP6Tcb5jAJqqLJpQHTv2k0zHBdTpP / rlG67uCtaM + nH8kbhkgGG3Vfd + fb8jvR0Un3oSeqHJuvqKDTp482VqBeKJEZi9AJTgn / pjcsHrPvL + NNaJoTZxRiYB8bsKNfyinczALNPKORvh / g4liXkC6P9UIV / Xwq1ier8vuu9YK6KfIBn0tqzXu5l8NuLwXR06M4RH0hxMDm4D8FUSEYNXT4nrQaf8K5oktfsC3qGaagxFezCoX6wls7pb5aUTXUtCjKNG4ikXRpbaJlr1AWEjgKdQjq1vIdESO8qlitYQQJCc4mJdEJ72r7cfAAHAsuesEaClwU =";
            return "AbqwVRbLNvKRB3HszUT2QfFAYccUBGTY9Bk3AQAQN3GXR2TbYjimL0tKdMsDQ7I+3XFZ7Qtzk9FMDzgy6GiEXa0p1+hrYJxKphGZVJdvawf1aR+2SmNUlwMw0kqpPkmtXRstOJgvqQtuD+6jv6EfkjrUuqhg782lFlikUG+vRg/dEWVOs6P6i4LGprRMEie5ODl2ed2wwRYUdvsmz+kBhQ3LsHNO6HyGNMtUNAEbKUIIIfZrWoBTH65MfvZSa+2OWosln77uFjhzz34V7trMHk6aIh3M7a2esEhO1ABfv5VYB698zH2R2Swqte3hUjZVf4YBgqEvyw3SDb31oTyROcaPauxPMESfj12YMSYRh3dmhcFrl3pMoe/bPO2wEyICekLf6MmPQSA4MBDRsMJoshe615iDlrESx4dlqkL+u4CumLyohd1VjisNhqtVUvoDc/aRDFOY3jWvf9+pqSF4R+HBj/ds39U/fLpTlYPex7yUfNzZEJC46du3A20SYwtKp+lYs0fN77YjjqqNq8Hx2buGNVM2aJLlJhGwObPAheHBiwljxSzCdAQR6upJbTmHyRO8WAn7wnr54RuNyslG2zViXl5JznvMUBAhVB33Y0jotQan4esE10zh2te4W/LjqWMCsHFKJ6IIIv2zIPdjO8lR/7C3sQLFG0usfP+sYNq8XN1NFJfDH75E9QgzpZvrcD2bLL8vp6sR+VLSmS66alXuAiPao1JblGqMbKWlJ5oviIzUw1uvxHVMAnU0MOHeLVnyENyWrFt+wkOxjRWE9CKvZ4/1ns1eJOVGGUWcsnVsFT6//pnfLWDvohI=";

            // return "--ENTER YOUR SCANDIT LICENSE KEY HERE--";
        }

        public void StartScanning()
        {
            StartScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void PauseScanning()
        {
            PauseScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void StopScanning()
        {
            StopScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void DidScan(string symbology, string code)
        {
            if (Delegate != null)
            {
                Delegate.DidScan(symbology, code);
            }
        }
    }
}
