# radau5-discontinuities-demo
A demo that demonstates issues with radau 5 integrating over a discontinuity.

See http://scicomp.stackexchange.com/questions/26182/failing-integration-with-the-radau5-implementation-in-dotnumerics-over-a-discon for more info.

To use this demo, open the solution in Visual Studio.Net and run either of the tests in radau5-discontinuities-demo/Model/ModelTests/DotNumericsTests.cs

These tests write to a trace file DotNumerics.Tests.txt in your temp directory.

In order to apply the fix, change line 

  if (THETA <= THET) goto LABEL20; 
  
in radau5-discontinuities-demo\DotNumerics\ODE\Radau5\radau5.cs
  
Which has comment // If this line is commented out, the Jacobian will be evaluated regularly and results are improved.
