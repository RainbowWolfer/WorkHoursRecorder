﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHoursRecorder.Models; 
public class Pair<T, W> {
	public T Key { get; set; }
	public W Value { get; set; }

	public Pair(T key, W value) {
		Key = key;
		Value = value;
	}
}
