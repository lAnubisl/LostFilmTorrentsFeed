// <copyright file="Usings.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
global using System;
global using System.IO;
global using System.Net;
global using System.Text.Json;
global using System.Threading.Tasks;
global using Azure.Data.Tables;
global using Azure.Storage.Blobs;
global using LostFilmMonitoring.BLL;
global using LostFilmMonitoring.BLL.Commands;
global using LostFilmMonitoring.BLL.Interfaces;
global using LostFilmMonitoring.BLL.Models.Request;
global using LostFilmMonitoring.BLL.Models.Response;
global using LostFilmMonitoring.BLL.Validators;
global using LostFilmMonitoring.Common;
global using LostFilmMonitoring.DAO.Azure;
global using LostFilmMonitoring.DAO.Interfaces;
global using LostFilmTV.Client;
global using LostFilmTV.Client.RssFeed;
global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
global using Microsoft.Azure.Functions.Worker.Http;
global using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
global using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

#pragma warning restore SA1200 // Using directives should be placed correctly
