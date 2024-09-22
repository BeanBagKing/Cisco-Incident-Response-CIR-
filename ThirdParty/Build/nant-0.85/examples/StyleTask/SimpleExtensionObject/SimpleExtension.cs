// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
   
//Calculates the circumference of a circle given the radius.
public class Calculate {

   private double circ = 0;
      
   public double Circumference(double radius){
      circ = Math.PI*2*radius;
      return circ;
   }
}