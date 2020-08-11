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
  class Student
  {
    public Student(int studentNo, string studentName, string studentSex)
    {
      StudentNo = studentNo;
      StudentName = studentName;
      StudentSex = studentSex;
    }
    public Student(int studentNo, string studentName, string studentSex, int studentPhoneNum, string homeNumber, string student_habit, bool is_Student, int student_Count, int Lucky_number)
    {
      StudentNo = studentNo;
      StudentName = studentName;
      StudentSex = studentSex;
      StudentPhoneNum = studentPhoneNum;
      HomeNumber = homeNumber;
      Is_Student = is_Student;
      Student_habit = student_habit;
      Student_Count = student_Count;
      Lucky_Number = Lucky_number;
    }
    public int StudentNo { get; set; }
    public string StudentName { get; set; }
    public string StudentSex { get; set; }
    public int StudentPhoneNum { get; set; }
    public string HomeNumber { get; set; }
    public string Student_habit { get; set; }
    public bool Is_Student { get; set; }
    public int Student_Count { get; set; }
    public int Lucky_Number { get; set; }
  }
}

