// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityTests
{
    public class Student
    {
        public Student(int studentNumber, string name, string sex)
        {
            StudentNumber = studentNumber;
            Name = name;
            Sex = sex;
        }

        public Student(int studentNumber, string name, string sex, int phoneNumber, string homeNumber, string hobby, bool isStudent, int count, int luckyNumber)
        {
            StudentNumber = studentNumber;
            Name = name;
            Sex = sex;
            PhoneNumber = phoneNumber;
            HomeNumber = homeNumber;
            IsStudent = isStudent;
            Hobby = hobby;
            Count = count;
            LuckyNumber = luckyNumber;
        }

        public int StudentNumber { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        public int PhoneNumber { get; set; }

        public string HomeNumber { get; set; }

        public string Hobby { get; set; }

        public bool IsStudent { get; set; }

        public int Count { get; set; }

        public int LuckyNumber { get; set; }
    }
}
