//
// literal.cs: Literal representation for the IL tree.
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) 2001 Ximian, Inc.
//

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CIR {
	public abstract class Literal : Expression {
		// <summary>
		//   This is different from ToString in that ToString
		//   is supposed to be there for debugging purposes,
		//   and is not guarantee to be useful for anything else,
		//   AsString() will provide something that can be used
		//   for round-tripping C# code.  Maybe it can be used
		//   for IL assembly as well.
		// </summary>
		public abstract string AsString ();

		override public string ToString ()
		{
			return AsString ();
		}

		static public string descape (char c)
		{
			switch (c){
			case '\a':
				return "\\a"; 
			case '\b':
				return "\\b"; 
			case '\n':
				return "\\n"; 
			case '\t':
				return "\\t"; 
			case '\v':
				return "\\v"; 
			case '\r':
				return "\\r"; 
			case '\\':
				return "\\\\";
			case '\f':
				return "\\f"; 
			case '\0':
				return "\\0"; 
			case '"':
				return "\\\""; 
			case '\'':
				return "\\\'"; 
			}
			return c.ToString ();
		}
	}

	public class NullLiteral : Literal {
		public NullLiteral ()
		{
		}
		
		override public string AsString ()
		{
			return "null";
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.object_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			ec.ig.Emit (OpCodes.Ldnull);
		}
	}

	public class BoolLiteral : Literal {
		bool val;
		
		public BoolLiteral (bool val)
		{
			this.val = val;
		}

		override public string AsString ()
		{
			return val ? "true" : "false";
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.bool_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			if (val)
				ec.ig.Emit (OpCodes.Ldc_I4_1);
			else
				ec.ig.Emit (OpCodes.Ldc_I4_0);
		}
	}

	public class CharLiteral : Literal {
		char c;
		
		public CharLiteral (char c)
		{
			this.c = c;
		}

		override public string AsString ()
		{
			return "\"" + descape (c) + "\"";
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.char_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			IntLiteral.EmitInt (ec.ig, c);
		}
	}

	public class IntLiteral : Literal {
		int i;

		public IntLiteral (int l)
		{
			i = l;
		}

		override public string AsString ()
		{
			return i.ToString ();
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.int32_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			ILGenerator ig = ec.ig;

			EmitInt (ig, i);
		}

		static public void EmitInt (ILGenerator ig, int i)
		{
			switch (i){
			case -1:
				ig.Emit (OpCodes.Ldc_I4_M1);
				break;
				
			case 0:
				ig.Emit (OpCodes.Ldc_I4_0);
				break;
				
			case 1:
				ig.Emit (OpCodes.Ldc_I4_1);
				break;
				
			case 2:
				ig.Emit (OpCodes.Ldc_I4_2);
				break;
				
			case 3:
				ig.Emit (OpCodes.Ldc_I4_3);
				break;
				
			case 4:
				ig.Emit (OpCodes.Ldc_I4_4);
				break;
				
			case 5:
				ig.Emit (OpCodes.Ldc_I4_5);
				break;
				
			case 6:
				ig.Emit (OpCodes.Ldc_I4_6);
				break;
				
			case 7:
				ig.Emit (OpCodes.Ldc_I4_7);
				break;
				
			case 8:
				ig.Emit (OpCodes.Ldc_I4_8);
				break;

			default:
				if (i < 255)
					ig.Emit (OpCodes.Ldc_I4_S, i);
				else
					ig.Emit (OpCodes.Ldc_I4, i);
				break;
			}
		}
	}

	public class FloatLiteral : Literal {
		float f;

		public FloatLiteral (float f)
		{
			this.f = f;
		}

		override public string AsString ()
		{
			return f.ToString ();
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.float_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			ec.ig.Emit (OpCodes.Ldc_R4, f);
		}
	}

	public class DoubleLiteral : Literal {
		double d;

		public DoubleLiteral (double d)
		{
			this.d = d;
		}

		override public string AsString ()
		{
			return d.ToString ();
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.double_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			ec.ig.Emit (OpCodes.Ldc_R8, d);
		}
	}

	public class DecimalLiteral : Literal {
		decimal d;

		public DecimalLiteral (decimal d)
		{
			this.d = d;
		}

		override public string AsString ()
		{
			return d.ToString ();
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.decimal_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
		}
	}

	public class StringLiteral : Literal {
		string s;

		public StringLiteral (string s)
		{
			this.s = s;
		}

		// FIXME: Escape the string.
		override public string AsString ()
		{
			return "\"" + s + "\"";
		}

		public override Expression Resolve (TypeContainer tc)
		{
			eclass = ExprClass.Value;
			type = TypeManager.string_type;

			return this;
		}

		public override void Emit (EmitContext ec)
		{
			ec.ig.Emit (OpCodes.Ldstr, s);
		}
	}
}
