// Copyright (C) 2010, 2011 Zeno Gantner
//
// This file is part of MyMediaLite.
//
// MyMediaLite is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MyMediaLite is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with MyMediaLite.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Data;
using System.Globalization;
using System.IO;
using MyMediaLite.Data;

namespace MyMediaLite.IO
{
	/// <summary>Class that offers static methods for reading in MovieLens 1M and 10M rating data</summary>
	/// <remarks>
	/// See http://www.grouplens.org/node/73#attachments and http://recsyswiki.com/wiki/MovieLens
	/// </remarks>
	public static class MovieLensRatingData
	{
		/// <summary>Read in rating data from a file</summary>
		/// <param name="filename">the name of the file to read from, "-" if STDIN</param>
		/// <param name="user_mapping">mapping object for user IDs</param>
		/// <param name="item_mapping">mapping object for item IDs</param>
		/// <returns>the rating data</returns>
		static public ITimedRatings Read(string filename, IEntityMapping user_mapping, IEntityMapping item_mapping)
		{
			try
			{
				using ( var reader = new StreamReader(filename) )
					return Read(reader, user_mapping, item_mapping);
			}
			catch (IOException e)
			{
				throw new IOException(string.Format("Could not read file {0}: {1}", filename, e.Message));
			}
		}

		/// <summary>Read in rating data from a TextReader</summary>
		/// <param name="reader">the <see cref="TextReader"/> to read from</param>
		/// <param name="user_mapping">mapping object for user IDs</param>
		/// <param name="item_mapping">mapping object for item IDs</param>
		/// <returns>the rating data</returns>
		static public ITimedRatings
			Read(TextReader reader,	IEntityMapping user_mapping, IEntityMapping item_mapping)
		{
			var ratings = new TimedRatings();

			var separators = new string[] { "::" };
			string line;

			while ((line = reader.ReadLine()) != null)
			{
				string[] tokens = line.Split(separators, StringSplitOptions.None);

				if (tokens.Length < 4)
					throw new IOException("Expected at least 4 columns: " + line);

				int user_id = user_mapping.ToInternalID(long.Parse(tokens[0]));
				int item_id = item_mapping.ToInternalID(long.Parse(tokens[1]));
				double rating = double.Parse(tokens[2], CultureInfo.InvariantCulture);
				DateTime time = DateTime.FromBinary(long.Parse(tokens[3]));

				ratings.Add(user_id, item_id, rating, time);
			}
			return ratings;
		}
	}
}