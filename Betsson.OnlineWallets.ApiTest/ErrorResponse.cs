﻿namespace Betsson.OnlineWallets.ApiTest
{
    internal class ErrorResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public string TraceId { get; set; }
    }
}