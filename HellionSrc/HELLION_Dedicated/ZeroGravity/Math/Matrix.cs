// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Math.Matrix
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Text.RegularExpressions;

namespace ZeroGravity.Math
{
  public class Matrix
  {
    private double detOfP = 1.0;
    public int rows;
    public int cols;
    public double[,] mat;
    public Matrix L;
    public Matrix U;
    private int[] pi;

    public Matrix(int iRows, int iCols)
    {
      this.rows = iRows;
      this.cols = iCols;
      this.mat = new double[this.rows, this.cols];
      for (int index1 = 0; index1 < this.rows; ++index1)
      {
        for (int index2 = 0; index2 < this.cols; ++index2)
          this.mat[index1, index2] = 0.0;
      }
    }

    public bool IsSquare()
    {
      return this.rows == this.cols;
    }

    public double this[int iRow, int iCol]
    {
      get
      {
        return this.mat[iRow, iCol];
      }
      set
      {
        this.mat[iRow, iCol] = value;
      }
    }

    public Matrix GetCol(int k)
    {
      Matrix matrix = new Matrix(this.rows, 1);
      for (int index = 0; index < this.rows; ++index)
        matrix[index, 0] = this.mat[index, k];
      return matrix;
    }

    public void SetCol(Matrix v, int k)
    {
      for (int index = 0; index < this.rows; ++index)
        this.mat[index, k] = v[index, 0];
    }

    public void MakeLU()
    {
      if (!this.IsSquare())
        throw new MException("The matrix is not square!");
      this.L = Matrix.IdentityMatrix(this.rows, this.cols);
      this.U = this.Duplicate();
      this.pi = new int[this.rows];
      for (int index = 0; index < this.rows; ++index)
        this.pi[index] = index;
      int index1 = 0;
      for (int index2 = 0; index2 < this.cols - 1; ++index2)
      {
        double num1 = 0.0;
        for (int index3 = index2; index3 < this.rows; ++index3)
        {
          if (System.Math.Abs(this.U[index3, index2]) > num1)
          {
            num1 = System.Math.Abs(this.U[index3, index2]);
            index1 = index3;
          }
        }
        if (num1 == 0.0)
          throw new MException("The matrix is singular!");
        int num2 = this.pi[index2];
        this.pi[index2] = this.pi[index1];
        this.pi[index1] = num2;
        for (int index3 = 0; index3 < index2; ++index3)
        {
          double num3 = this.L[index2, index3];
          this.L[index2, index3] = this.L[index1, index3];
          this.L[index1, index3] = num3;
        }
        if (index2 != index1)
          this.detOfP = this.detOfP * -1.0;
        for (int index3 = 0; index3 < this.cols; ++index3)
        {
          double num3 = this.U[index2, index3];
          this.U[index2, index3] = this.U[index1, index3];
          this.U[index1, index3] = num3;
        }
        for (int index3 = index2 + 1; index3 < this.rows; ++index3)
        {
          this.L[index3, index2] = this.U[index3, index2] / this.U[index2, index2];
          for (int index4 = index2; index4 < this.cols; ++index4)
            this.U[index3, index4] = this.U[index3, index4] - this.L[index3, index2] * this.U[index2, index4];
        }
      }
    }

    public Matrix SolveWith(Matrix v)
    {
      if (this.rows != this.cols)
        throw new MException("The matrix is not square!");
      if (this.rows != v.rows)
        throw new MException("Wrong number of results in solution vector!");
      if (this.L == null)
        this.MakeLU();
      Matrix b = new Matrix(this.rows, 1);
      for (int index = 0; index < this.rows; ++index)
        b[index, 0] = v[this.pi[index], 0];
      return Matrix.SubsBack(this.U, Matrix.SubsForth(this.L, b));
    }

    public Matrix Invert()
    {
      if (this.L == null)
        this.MakeLU();
      Matrix matrix = new Matrix(this.rows, this.cols);
      for (int k = 0; k < this.rows; ++k)
      {
        Matrix v1 = Matrix.ZeroMatrix(this.rows, 1);
        v1[k, 0] = 1.0;
        Matrix v2 = this.SolveWith(v1);
        matrix.SetCol(v2, k);
      }
      return matrix;
    }

    public double Det()
    {
      if (this.L == null)
        this.MakeLU();
      double detOfP = this.detOfP;
      for (int index = 0; index < this.rows; ++index)
        detOfP *= this.U[index, index];
      return detOfP;
    }

    public Matrix GetP()
    {
      if (this.L == null)
        this.MakeLU();
      Matrix matrix = Matrix.ZeroMatrix(this.rows, this.cols);
      for (int index = 0; index < this.rows; ++index)
        matrix[this.pi[index], index] = 1.0;
      return matrix;
    }

    public Matrix Duplicate()
    {
      Matrix matrix = new Matrix(this.rows, this.cols);
      for (int index1 = 0; index1 < this.rows; ++index1)
      {
        for (int index2 = 0; index2 < this.cols; ++index2)
          matrix[index1, index2] = this.mat[index1, index2];
      }
      return matrix;
    }

    public static Matrix SubsForth(Matrix A, Matrix b)
    {
      if (A.L == null)
        A.MakeLU();
      int rows = A.rows;
      Matrix matrix1 = new Matrix(rows, 1);
      for (int index1 = 0; index1 < rows; ++index1)
      {
        matrix1[index1, 0] = b[index1, 0];
        for (int index2 = 0; index2 < index1; ++index2)
        {
          Matrix matrix2 = matrix1;
          int index3 = index1;
          matrix2[index3, 0] = matrix2[index3, 0] - A[index1, index2] * matrix1[index2, 0];
        }
        matrix1[index1, 0] = matrix1[index1, 0] / A[index1, index1];
      }
      return matrix1;
    }

    public static Matrix SubsBack(Matrix A, Matrix b)
    {
      if (A.L == null)
        A.MakeLU();
      int rows = A.rows;
      Matrix matrix1 = new Matrix(rows, 1);
      for (int index1 = rows - 1; index1 > -1; --index1)
      {
        matrix1[index1, 0] = b[index1, 0];
        for (int index2 = rows - 1; index2 > index1; --index2)
        {
          Matrix matrix2 = matrix1;
          int index3 = index1;
          matrix2[index3, 0] = matrix2[index3, 0] - A[index1, index2] * matrix1[index2, 0];
        }
        matrix1[index1, 0] = matrix1[index1, 0] / A[index1, index1];
      }
      return matrix1;
    }

    public static Matrix ZeroMatrix(int iRows, int iCols)
    {
      Matrix matrix = new Matrix(iRows, iCols);
      for (int index1 = 0; index1 < iRows; ++index1)
      {
        for (int index2 = 0; index2 < iCols; ++index2)
          matrix[index1, index2] = 0.0;
      }
      return matrix;
    }

    public static Matrix IdentityMatrix(int iRows, int iCols)
    {
      Matrix matrix = Matrix.ZeroMatrix(iRows, iCols);
      for (int index = 0; index < System.Math.Min(iRows, iCols); ++index)
        matrix[index, index] = 1.0;
      return matrix;
    }

    public static Matrix RandomMatrix(int iRows, int iCols, int dispersion)
    {
      Random random = new Random();
      Matrix matrix = new Matrix(iRows, iCols);
      for (int index1 = 0; index1 < iRows; ++index1)
      {
        for (int index2 = 0; index2 < iCols; ++index2)
          matrix[index1, index2] = (double) random.Next(-dispersion, dispersion);
      }
      return matrix;
    }

    public static Matrix Parse(string ps)
    {
      string[] strArray1 = Regex.Split(Matrix.NormalizeMatrixString(ps), "\r\n");
      string[] strArray2 = strArray1[0].Split(' ');
      Matrix matrix = new Matrix(strArray1.Length, strArray2.Length);
      try
      {
        for (int index1 = 0; index1 < strArray1.Length; ++index1)
        {
          string[] strArray3 = strArray1[index1].Split(' ');
          for (int index2 = 0; index2 < strArray3.Length; ++index2)
            matrix[index1, index2] = double.Parse(strArray3[index2]);
        }
      }
      catch (FormatException ex)
      {
        throw new MException("Wrong input format!");
      }
      return matrix;
    }

    public override string ToString()
    {
      string str = "";
      for (int index1 = 0; index1 < this.rows; ++index1)
      {
        for (int index2 = 0; index2 < this.cols; ++index2)
          str = str + string.Format("{0,5:0.00}", (object) this.mat[index1, index2]) + " ";
        str += "\r\n";
      }
      return str;
    }

    public static Matrix Transpose(Matrix m)
    {
      Matrix matrix = new Matrix(m.cols, m.rows);
      for (int index1 = 0; index1 < m.rows; ++index1)
      {
        for (int index2 = 0; index2 < m.cols; ++index2)
          matrix[index2, index1] = m[index1, index2];
      }
      return matrix;
    }

    public static Matrix Power(Matrix m, int pow)
    {
      if (pow == 0)
        return Matrix.IdentityMatrix(m.rows, m.cols);
      if (pow == 1)
        return m.Duplicate();
      if (pow == -1)
        return m.Invert();
      Matrix matrix1;
      if (pow < 0)
      {
        matrix1 = m.Invert();
        pow *= -1;
      }
      else
        matrix1 = m.Duplicate();
      Matrix matrix2 = Matrix.IdentityMatrix(m.rows, m.cols);
      while ((uint) pow > 0U)
      {
        if ((pow & 1) == 1)
          matrix2 *= matrix1;
        matrix1 *= matrix1;
        pow >>= 1;
      }
      return matrix2;
    }

    private static void SafeAplusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
    {
      for (int index1 = 0; index1 < size; ++index1)
      {
        for (int index2 = 0; index2 < size; ++index2)
        {
          C[index1, index2] = 0.0;
          if (xa + index2 < A.cols && ya + index1 < A.rows)
          {
            Matrix matrix = C;
            int index3 = index1;
            int index4 = index2;
            matrix[index3, index4] = matrix[index3, index4] + A[ya + index1, xa + index2];
          }
          if (xb + index2 < B.cols && yb + index1 < B.rows)
          {
            Matrix matrix = C;
            int index3 = index1;
            int index4 = index2;
            matrix[index3, index4] = matrix[index3, index4] + B[yb + index1, xb + index2];
          }
        }
      }
    }

    private static void SafeAminusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
    {
      for (int index1 = 0; index1 < size; ++index1)
      {
        for (int index2 = 0; index2 < size; ++index2)
        {
          C[index1, index2] = 0.0;
          if (xa + index2 < A.cols && ya + index1 < A.rows)
          {
            Matrix matrix = C;
            int index3 = index1;
            int index4 = index2;
            matrix[index3, index4] = matrix[index3, index4] + A[ya + index1, xa + index2];
          }
          if (xb + index2 < B.cols && yb + index1 < B.rows)
          {
            Matrix matrix = C;
            int index3 = index1;
            int index4 = index2;
            matrix[index3, index4] = matrix[index3, index4] - B[yb + index1, xb + index2];
          }
        }
      }
    }

    private static void SafeACopytoC(Matrix A, int xa, int ya, Matrix C, int size)
    {
      for (int index1 = 0; index1 < size; ++index1)
      {
        for (int index2 = 0; index2 < size; ++index2)
        {
          C[index1, index2] = 0.0;
          if (xa + index2 < A.cols && ya + index1 < A.rows)
          {
            Matrix matrix = C;
            int index3 = index1;
            int index4 = index2;
            matrix[index3, index4] = matrix[index3, index4] + A[ya + index1, xa + index2];
          }
        }
      }
    }

    private static void AplusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
    {
      for (int index1 = 0; index1 < size; ++index1)
      {
        for (int index2 = 0; index2 < size; ++index2)
          C[index1, index2] = A[ya + index1, xa + index2] + B[yb + index1, xb + index2];
      }
    }

    private static void AminusBintoC(Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size)
    {
      for (int index1 = 0; index1 < size; ++index1)
      {
        for (int index2 = 0; index2 < size; ++index2)
          C[index1, index2] = A[ya + index1, xa + index2] - B[yb + index1, xb + index2];
      }
    }

    private static void ACopytoC(Matrix A, int xa, int ya, Matrix C, int size)
    {
      for (int index1 = 0; index1 < size; ++index1)
      {
        for (int index2 = 0; index2 < size; ++index2)
          C[index1, index2] = A[ya + index1, xa + index2];
      }
    }

    private static Matrix StrassenMultiply(Matrix A, Matrix B)
    {
      if (A.cols != B.rows)
        throw new MException("Wrong dimension of matrix!");
      int num1 = System.Math.Max(System.Math.Max(A.rows, A.cols), System.Math.Max(B.rows, B.cols));
      if (num1 < 32)
      {
        Matrix matrix1 = Matrix.ZeroMatrix(A.rows, B.cols);
        for (int index1 = 0; index1 < matrix1.rows; ++index1)
        {
          for (int index2 = 0; index2 < matrix1.cols; ++index2)
          {
            for (int index3 = 0; index3 < A.cols; ++index3)
            {
              Matrix matrix2 = matrix1;
              int index4 = index1;
              int index5 = index2;
              matrix2[index4, index5] = matrix2[index4, index5] + A[index1, index3] * B[index3, index2];
            }
          }
        }
        return matrix1;
      }
      int num2 = 1;
      int length = 0;
      while (num1 > num2)
      {
        num2 *= 2;
        ++length;
      }
      int num3 = num2 / 2;
      Matrix[,] f = new Matrix[length, 9];
      for (int index1 = 0; index1 < length - 4; ++index1)
      {
        int num4 = (int) System.Math.Pow(2.0, (double) (length - index1 - 1));
        for (int index2 = 0; index2 < 9; ++index2)
          f[index1, index2] = new Matrix(num4, num4);
      }
      Matrix.SafeAplusBintoC(A, 0, 0, A, num3, num3, f[0, 0], num3);
      Matrix.SafeAplusBintoC(B, 0, 0, B, num3, num3, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 2], 1, f);
      Matrix.SafeAplusBintoC(A, 0, num3, A, num3, num3, f[0, 0], num3);
      Matrix.SafeACopytoC(B, 0, 0, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 3], 1, f);
      Matrix.SafeACopytoC(A, 0, 0, f[0, 0], num3);
      Matrix.SafeAminusBintoC(B, num3, 0, B, num3, num3, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 4], 1, f);
      Matrix.SafeACopytoC(A, num3, num3, f[0, 0], num3);
      Matrix.SafeAminusBintoC(B, 0, num3, B, 0, 0, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 5], 1, f);
      Matrix.SafeAplusBintoC(A, 0, 0, A, num3, 0, f[0, 0], num3);
      Matrix.SafeACopytoC(B, num3, num3, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 6], 1, f);
      Matrix.SafeAminusBintoC(A, 0, num3, A, 0, 0, f[0, 0], num3);
      Matrix.SafeAplusBintoC(B, 0, 0, B, num3, 0, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 7], 1, f);
      Matrix.SafeAminusBintoC(A, num3, 0, A, num3, num3, f[0, 0], num3);
      Matrix.SafeAplusBintoC(B, 0, num3, B, num3, num3, f[0, 1], num3);
      Matrix.StrassenMultiplyRun(f[0, 0], f[0, 1], f[0, 8], 1, f);
      Matrix matrix = new Matrix(A.rows, B.cols);
      for (int index1 = 0; index1 < System.Math.Min(num3, matrix.rows); ++index1)
      {
        for (int index2 = 0; index2 < System.Math.Min(num3, matrix.cols); ++index2)
          matrix[index1, index2] = f[0, 2][index1, index2] + f[0, 5][index1, index2] - f[0, 6][index1, index2] + f[0, 8][index1, index2];
      }
      for (int index1 = 0; index1 < System.Math.Min(num3, matrix.rows); ++index1)
      {
        for (int index2 = num3; index2 < System.Math.Min(2 * num3, matrix.cols); ++index2)
          matrix[index1, index2] = f[0, 4][index1, index2 - num3] + f[0, 6][index1, index2 - num3];
      }
      for (int index1 = num3; index1 < System.Math.Min(2 * num3, matrix.rows); ++index1)
      {
        for (int index2 = 0; index2 < System.Math.Min(num3, matrix.cols); ++index2)
          matrix[index1, index2] = f[0, 3][index1 - num3, index2] + f[0, 5][index1 - num3, index2];
      }
      for (int index1 = num3; index1 < System.Math.Min(2 * num3, matrix.rows); ++index1)
      {
        for (int index2 = num3; index2 < System.Math.Min(2 * num3, matrix.cols); ++index2)
          matrix[index1, index2] = f[0, 2][index1 - num3, index2 - num3] - f[0, 3][index1 - num3, index2 - num3] + f[0, 4][index1 - num3, index2 - num3] + f[0, 7][index1 - num3, index2 - num3];
      }
      return matrix;
    }

    private static void StrassenMultiplyRun(Matrix A, Matrix B, Matrix C, int l, Matrix[,] f)
    {
      int rows = A.rows;
      int num = rows / 2;
      if (rows < 32)
      {
        for (int index1 = 0; index1 < C.rows; ++index1)
        {
          for (int index2 = 0; index2 < C.cols; ++index2)
          {
            C[index1, index2] = 0.0;
            for (int index3 = 0; index3 < A.cols; ++index3)
            {
              Matrix matrix = C;
              int index4 = index1;
              int index5 = index2;
              matrix[index4, index5] = matrix[index4, index5] + A[index1, index3] * B[index3, index2];
            }
          }
        }
      }
      else
      {
        Matrix.AplusBintoC(A, 0, 0, A, num, num, f[l, 0], num);
        Matrix.AplusBintoC(B, 0, 0, B, num, num, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 2], l + 1, f);
        Matrix.AplusBintoC(A, 0, num, A, num, num, f[l, 0], num);
        Matrix.ACopytoC(B, 0, 0, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 3], l + 1, f);
        Matrix.ACopytoC(A, 0, 0, f[l, 0], num);
        Matrix.AminusBintoC(B, num, 0, B, num, num, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 4], l + 1, f);
        Matrix.ACopytoC(A, num, num, f[l, 0], num);
        Matrix.AminusBintoC(B, 0, num, B, 0, 0, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 5], l + 1, f);
        Matrix.AplusBintoC(A, 0, 0, A, num, 0, f[l, 0], num);
        Matrix.ACopytoC(B, num, num, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 6], l + 1, f);
        Matrix.AminusBintoC(A, 0, num, A, 0, 0, f[l, 0], num);
        Matrix.AplusBintoC(B, 0, 0, B, num, 0, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 7], l + 1, f);
        Matrix.AminusBintoC(A, num, 0, A, num, num, f[l, 0], num);
        Matrix.AplusBintoC(B, 0, num, B, num, num, f[l, 1], num);
        Matrix.StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 8], l + 1, f);
        for (int index1 = 0; index1 < num; ++index1)
        {
          for (int index2 = 0; index2 < num; ++index2)
            C[index1, index2] = f[l, 2][index1, index2] + f[l, 5][index1, index2] - f[l, 6][index1, index2] + f[l, 8][index1, index2];
        }
        for (int index1 = 0; index1 < num; ++index1)
        {
          for (int index2 = num; index2 < rows; ++index2)
            C[index1, index2] = f[l, 4][index1, index2 - num] + f[l, 6][index1, index2 - num];
        }
        for (int index1 = num; index1 < rows; ++index1)
        {
          for (int index2 = 0; index2 < num; ++index2)
            C[index1, index2] = f[l, 3][index1 - num, index2] + f[l, 5][index1 - num, index2];
        }
        for (int index1 = num; index1 < rows; ++index1)
        {
          for (int index2 = num; index2 < rows; ++index2)
            C[index1, index2] = f[l, 2][index1 - num, index2 - num] - f[l, 3][index1 - num, index2 - num] + f[l, 4][index1 - num, index2 - num] + f[l, 7][index1 - num, index2 - num];
        }
      }
    }

    public static Matrix StupidMultiply(Matrix m1, Matrix m2)
    {
      if (m1.cols != m2.rows)
        throw new MException("Wrong dimensions of matrix!");
      Matrix matrix1 = Matrix.ZeroMatrix(m1.rows, m2.cols);
      for (int index1 = 0; index1 < matrix1.rows; ++index1)
      {
        for (int index2 = 0; index2 < matrix1.cols; ++index2)
        {
          for (int index3 = 0; index3 < m1.cols; ++index3)
          {
            Matrix matrix2 = matrix1;
            int index4 = index1;
            int index5 = index2;
            matrix2[index4, index5] = matrix2[index4, index5] + m1[index1, index3] * m2[index3, index2];
          }
        }
      }
      return matrix1;
    }

    private static Matrix Multiply(double n, Matrix m)
    {
      Matrix matrix = new Matrix(m.rows, m.cols);
      for (int index1 = 0; index1 < m.rows; ++index1)
      {
        for (int index2 = 0; index2 < m.cols; ++index2)
          matrix[index1, index2] = m[index1, index2] * n;
      }
      return matrix;
    }

    private static Matrix Add(Matrix m1, Matrix m2)
    {
      if (m1.rows != m2.rows || m1.cols != m2.cols)
        throw new MException("Matrices must have the same dimensions!");
      Matrix matrix = new Matrix(m1.rows, m1.cols);
      for (int index1 = 0; index1 < matrix.rows; ++index1)
      {
        for (int index2 = 0; index2 < matrix.cols; ++index2)
          matrix[index1, index2] = m1[index1, index2] + m2[index1, index2];
      }
      return matrix;
    }

    public static string NormalizeMatrixString(string matStr)
    {
      while (matStr.IndexOf("  ") != -1)
        matStr = matStr.Replace("  ", " ");
      matStr = matStr.Replace(" \r\n", "\r\n");
      matStr = matStr.Replace("\r\n ", "\r\n");
      matStr = matStr.Replace("\r\n", "|");
      while (matStr.LastIndexOf("|") == matStr.Length - 1)
        matStr = matStr.Substring(0, matStr.Length - 1);
      matStr = matStr.Replace("|", "\r\n");
      return matStr.Trim();
    }

    public static Matrix operator -(Matrix m)
    {
      return Matrix.Multiply(-1.0, m);
    }

    public static Matrix operator +(Matrix m1, Matrix m2)
    {
      return Matrix.Add(m1, m2);
    }

    public static Matrix operator -(Matrix m1, Matrix m2)
    {
      return Matrix.Add(m1, -m2);
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
      return Matrix.StrassenMultiply(m1, m2);
    }

    public static Matrix operator *(double n, Matrix m)
    {
      return Matrix.Multiply(n, m);
    }
  }
}
