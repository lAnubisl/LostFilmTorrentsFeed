// <copyright file="User.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.DomainModels
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets LostFilmCookie.
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// Gets or sets Usess.
        /// </summary>
        public string Usess { get; set; }

        /// <summary>
        /// Gets or sets Uid.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets user identifier which should be in link of torrent file announces.
        /// <![CDATA[http://bt.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
        ///          http://bt99.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
        ///          http://bt0.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
        ///          http://user5.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
        ///          http://user1.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce]]>
        /// Here '1b07a52cb12a12945e15cca756f83789' is the user id.
        /// </summary>
        public string TrackerId { get; set; }

        /// <summary>
        /// Gets or sets LastActivity.
        /// </summary>
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// Gets or sets Subscriptions.
        /// </summary>
        public List<Subscription> Subscriptions { get; set; }
    }
}
