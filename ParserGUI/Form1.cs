using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parser.Grammar;
using Parser.StateMachine;
using LexicalAnalysis.Definitions;
using LexicalAnalysis;
using Parser.Parsers;
using Parser.Definitions;
using System.Threading;
using System.IO;
using System.IO.Compression;
using Parser.Parsers.AllInOne;
using Parser;

namespace ParserGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new Thread(aio)).Start(txtCFG.Text);

            #region Parse Table Builder
            //List<dynamic> gridCollection = new List<dynamic>();

            //foreach (var item in table.ActionTable.Select(a => new
            //{
            //    State = a.Key.Row,
            //    Input = a.Key.Column,
            //    Value = a.Value
            //}))
            //{
            //    gridCollection.Add(item);
            //}

            //foreach (var item in table.GotoTable.Select(a => new
            //{
            //    State = a.Key.Row,
            //    Input = a.Key.Column,
            //    Value = a.Value
            //}))
            //{
            //    gridCollection.Add(item);
            //}
            ////grdTable.AutoGenerateColumns = true;

            //grdTable.Columns.Clear();

            //grdTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //foreach (dynamic column in gridCollection.OrderBy(a => a is Terminal<string>).Select(a => a.Input).DistinctBy(a => a.ToString()))
            //{
            //    DataGridViewColumn col = new DataGridViewTextBoxColumn();
            //    col.HeaderText = column.ToString();
            //    col.SortMode = DataGridViewColumnSortMode.NotSortable;
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //    grdTable.Columns.Add(col);
            //}


            //grdTable.Columns[grdTable.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //grdTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //var rows = gridCollection.GroupBy(a => a.State).Select(a => new List<dynamic>(a)).ToArray();
            //for (int i = 0; i < rows.Length; i++)
            //{
            //    List<dynamic> row = new List<dynamic>();
            //    for (int r = 0; r < grdTable.Columns.Count; r++)
            //    {

            //        if (r < rows[i].Count())
            //        {
            //            if (rows[i].Any(a =>
            //            {
            //                if (a != null)
            //                {
            //                    return a.Input.ToString().Equals(grdTable.Columns[r].HeaderText);
            //                }
            //                return false;
            //            }))
            //            {
            //                row.Add(concatArray(rows[i].First(a =>
            //                {
            //                    if (a != null)
            //                    {
            //                        return a.Input.ToString().Equals(grdTable.Columns[r].HeaderText);
            //                    }
            //                    return false;
            //                }).Value, "\n"));
            //            }
            //            else
            //            {
            //                row.Add(null);
            //                rows[i].Insert(r, null);
            //            }
            //        }
            //    }
            //    grdTable.Rows.Add(row.ToArray());
            //    grdTable.Rows[i].HeaderCell.Value = i.ToString();

            //}

            //foreach(DataGridViewColumn column in grdTable.Columns)
            //{
            //    int colw = column.Width;
            //    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //    column.Width = colw;
            //} 
            #endregion
        }

        IEnumerable<Token<string>> tokens;

        Lexer lexer;

        /// <summary>
        /// Concats each individual element in the given dynamic object to produce a single string representing all elements.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private string concatArray(dynamic a, string seperator = " ")
        {
            if (a is IEnumerable<dynamic>)
            {
                StringBuilder b = new StringBuilder();
                foreach (dynamic e in a)
                {
                    b.Append(e.ToString());
                    b.Append(seperator);
                }
                return b.ToString();
            }
            else
            {
                return a.ToString();
            }
        }

        AIOLRParser<string> aioParser;

        private static IEnumerable<ParserTokenDefinition<string>> GetKeywords(IEnumerable<string> keywords)
        {
            List<ParserTokenDefinition<string>> words = new List<ParserTokenDefinition<string>>(keywords.Count());
            foreach(string keyword in keywords)
            {
                words.Add(new KeywordParserTokenDefinition(keyword, true));
            }
            return words;
        }

        private void aio(object p)
        {
            string text = (string)p;

            #region Def
            ////// build the definitions
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //(
            //    new ParserTokenDefinitionCollection<string>
            //    (

            //        new List<ParserTokenDefinition<string>>
            //        {
            //            //map 'n' to a number, define that we should keep this terminal
            //            new StringedParserTokenDefinition(@"(?:\b|^)[\d]+(?:\b|$)", "n", true),
            //            //map '(', define that we should discard this terminal
            //            new StringedParserTokenDefinition(@"\(", "(", false),
            //            //map ')', define that we should discard this terminal
            //            new StringedParserTokenDefinition(@"\)", ")", false),
            //            //map '+', define that we should keep this terminal
            //            new StringedParserTokenDefinition(@"\+", "+", true),
            //            //map '*'
            //            new StringedParserTokenDefinition(@"\*", "*", false),
            //            //map '/'
            //            new StringedParserTokenDefinition(@"/|\\", "/", false),
            //            //map '-'
            //            new StringedParserTokenDefinition(@"\-", "-", false),
            //            //map '^'
            //            new StringedParserTokenDefinition(@"\^", "^", false)
            //        }
            //    ),
            //    new List<Production<string>>
            //    {
            //        //E -> T
            //        new Production<string>("E".ToNonTerminal<string>(), "T".ToNonTerminal<string>()),

            //        //T -> n
            //        //the 'n' terminal generated into another terminal which contains the properties to map to a Token with the same TokenType as this terminal's value
            //        //therefore 'n' will map to the number token definition defined above.
            //        //since 'n'(Terminal) == 'n'(TokenDefinition).TokenType then 'n' maps to Terminal<Token<string>>((new Token(0, 'n', null).ToTerminal(keep, where evaluated token.TokenType == 'n')
            //        new Production<string>("T".ToNonTerminal<string>(), "n".ToTerminal()),

            //        //E -> ( E )
            //        new Production<string>("E".ToNonTerminal<string>(), "(".ToTerminal(), "E".ToNonTerminal<string>(), ")".ToTerminal()),

            //        //E -> E + T
            //        new Production<string>("E".ToNonTerminal<string>(), "E".ToNonTerminal<string>(), "+".ToTerminal(), "T".ToNonTerminal<string>()),

            //        ////M -> E * n
            //        //new Production<string>("M".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "*".ToTerminal(), "n".ToTerminal()),

            //        ////D -> E / n
            //        //new Production<string>("D".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "/".ToTerminal(), "n".ToTerminal()),

            //        ////S -> E - n
            //        //new Production<string>("S".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "-".ToTerminal(), "n".ToTerminal()),

            //        ////P -> E ^ n
            //        //new Production<string>("P".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "^".ToTerminal(), "n".ToTerminal()),

            //        //////E -> A
            //        ////new Production<string>("E".ToNonTerminal<string>(false), "A".ToNonTerminal<string>()),

            //        ////E -> M
            //        //new Production<string>("E".ToNonTerminal<string>(false), "M".ToNonTerminal<string>()),

            //        ////E -> D
            //        //new Production<string>("E".ToNonTerminal<string>(false), "D".ToNonTerminal<string>()),

            //        ////E -> S
            //        //new Production<string>("E".ToNonTerminal<string>(false), "S".ToNonTerminal<string>()),
            //    }
            //);
            #endregion

            #region Def
            //C# Definition
            ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            (
                new ParserTokenDefinitionCollection<string>
                (
                    new List<ParserTokenDefinition<string>>
                    {
                        #region Keywords
		                //Matches to the 'public' keyword, we should keep this keyword.
                        new KeywordParserTokenDefinition("public", true),
                        //Matches to the 'private' keyword, we should keep this keyword.
                        new KeywordParserTokenDefinition("private", true),
                        //Matches to the 'protected' keyword, we should keep this keyword.
                        new KeywordParserTokenDefinition("protected", true),
                        new KeywordParserTokenDefinition("internal", true),

                        //Matches to the 'void' keyword
                        new KeywordParserTokenDefinition("void", true),
                        
                        //Matches to the 'null' keyword
                        new KeywordParserTokenDefinition("null", true),
                        new KeywordParserTokenDefinition("bool", true),
                        new KeywordParserTokenDefinition("int", true),
                        new KeywordParserTokenDefinition("short", true),
                        new KeywordParserTokenDefinition("float", true),
                        new KeywordParserTokenDefinition("double", true),
                        new KeywordParserTokenDefinition("decimal", true),
                        new KeywordParserTokenDefinition("uint", true),
                        new KeywordParserTokenDefinition("ulong", true),
                        new KeywordParserTokenDefinition("long", true),
                        new KeywordParserTokenDefinition("byte", true),
                        new KeywordParserTokenDefinition("sbyte", true),
                        new KeywordParserTokenDefinition("char", true),
                        new KeywordParserTokenDefinition("string", true),

                        //Matches to the 'class' keyword
                        new KeywordParserTokenDefinition("class", true),
                        new KeywordParserTokenDefinition("struct", true),
                        new KeywordParserTokenDefinition("enum", true),
                        new KeywordParserTokenDefinition("interface", true),
                        new KeywordParserTokenDefinition("abstract", true),
                        new KeywordParserTokenDefinition("sealed", true),

                        //Matches to the 'true' keyword
                        new KeywordParserTokenDefinition("true", true),
                        new KeywordParserTokenDefinition("false", true),

                        //Matches to the 'as' keyword
                        new KeywordParserTokenDefinition("as", true),
                        new KeywordParserTokenDefinition("is", true),

                        //Matches to the 'if' keyword
                        new KeywordParserTokenDefinition("if", true),
                        new KeywordParserTokenDefinition("else", true),
                        new KeywordParserTokenDefinition("do", true),
                        new KeywordParserTokenDefinition("while", true),
                        new KeywordParserTokenDefinition("break", true),
                        new KeywordParserTokenDefinition("for", true),
                        new KeywordParserTokenDefinition("foreach", true),
                        new KeywordParserTokenDefinition("try", true),
                        new KeywordParserTokenDefinition("catch", true),
                        new KeywordParserTokenDefinition("finally", true),
                        new KeywordParserTokenDefinition("switch", true),
                        new KeywordParserTokenDefinition("case", true),
                        new KeywordParserTokenDefinition("goto", true),
                        new KeywordParserTokenDefinition("continue", true),

                        //Matches to the 'return' keyword
                        new KeywordParserTokenDefinition("return", true), 

                        //Matches to the 'delegate' keyword
                        new KeywordParserTokenDefinition("delegate", true),

                        //Matches to the 'static' keyword
                        new KeywordParserTokenDefinition("static", true),
                        //Matches to the 'const' keyword
                        new KeywordParserTokenDefinition("const", true),
	                    #endregion

                        //Matches to a word of arbitrary length as an identifier, we should keep this.
                        new StringedParserTokenDefinition(@"\b\w+\b", "Id", true),
                        //matches to an opening parenthese, we should discard this token.
                        new StringedParserTokenDefinition(@"\(","(", false),
                        //matches to an closing parenthese, we should discard this token.
                        new StringedParserTokenDefinition(@"\)", ")", false),
                        //matches to a semicolin, we should discard this token.
                        new StringedParserTokenDefinition(@";", ";", false),
                        //matches to an opening curly-brace, we should discard this token.
                        new StringedParserTokenDefinition(@"\{", "{", false),
                        //matches to a closing curly-brace, we should discard this token.
                        new StringedParserTokenDefinition(@"\}", "}", false),
                        //matches to a comma, we should discard this token
                        new StringedParserTokenDefinition(@",", ",", false),
                        //matches to the plus sign, we should keep this token
                        new StringedParserTokenDefinition(@"\+", "+", true),
                        //matches to the minus sign, we should keep this token
                        new StringedParserTokenDefinition(@"\-", "-", true),
                        //matches to the times sign, we should keep this token
                        new StringedParserTokenDefinition(@"\*", "*", true),
                        //matches to the division sign, we should keep this token
                        new StringedParserTokenDefinition(@"/", "/", true),
                        //matches to the assignment operator, we should keep this token
                        new StringedParserTokenDefinition(@"=", "=", true),
                    }
                ),
                new List<Production<string>>
                {
                    //The C# language structure is split into two parts:
                    //1. The program definitions (class definitions, method defintions, field definitions, i.e. compile-time constants)
                    //2. The program runtime (statements that manipulate variables, fuction calls, i.e. the things that are evaluated at runtime)
                    #region C# Context-Free-Grammar


                    #region Program Definitions
                    
                    //TypeName -> Id GenericTemplate
                    new Production<string>("TypeName".ToNonTerminal(), "Id".ToTerminal(), "GenericTemplate".ToNonTerminal()),

		            //Class -> ClassAccessMod class Id GenericTemplate { MemberLst }
                    new Production<string>("Class".ToNonTerminal(), "ClassAccessMod".ToNonTerminal(), "class".ToTerminal(), "Id".ToTerminal(), "GenericTemplate".ToNonTerminal(), "{".ToTerminal(), "MemberLst".ToNonTerminal(), "}".ToTerminal()),

                    #region Generics Definitions
                    //Defines the productions that implement generics(i.e. type parameters)

		            //GenericTemplate -> < GenericLst >
                    new Production<string>("GenericTemplate".ToNonTerminal(), "<".ToTerminal(), "GenericLst".ToNonTerminal(), ">".ToTerminal()),

                    //GenericTemplate -> nothing
                    new Production<string>("GenericTemplate".ToNonTerminal()),

                    //GenericLst -> Id
                    new Production<string>("GenericLst".ToNonTerminal(), "Id".ToTerminal()),

                    //GenericLst -> GenericLst , Id
                    new Production<string>("GenericLst".ToNonTerminal(), "GenericLst".ToNonTerminal(), "Id".ToTerminal()), 
	                #endregion

                    #region Inheritance Definitions
                    //Defines the productions for a list of types to inherit from(e.g. className : IEnumerable)

		            //InheritanceDef -> : InheritanceLst
                    new Production<string>("InheritanceDef".ToNonTerminal(), ":".ToTerminal(), "InheritanceLst".ToNonTerminal()),

                    //InheritanceLst -> TypeName
                    new Production<string>("InheritanceLst".ToNonTerminal(), "TypeName".ToNonTerminal()),

                    //InheritanceLst -> InheritanceLst , TypeName
                    new Production<string>("InheritanceLst".ToNonTerminal(), "InheritanceLst".ToNonTerminal(), ",".ToTerminal(), "TypeName".ToNonTerminal()), 
	                #endregion

                    #region Class Access Modifers
                    //Defines the access modifiers that can be used on a class(i.e. public and internal)

		            //ClassAccessMod -> public | internal
                    new Production<string>("ClassAccessMod".ToNonTerminal(), "public".ToTerminal()),
                    new Production<string>("ClassAccessMod".ToNonTerminal(), "internal".ToNonTerminal()),

                    //ClassAccessMod -> nothing
                    new Production<string>("ClassAccessMod".ToNonTerminal()), 
	                #endregion

                    #region Member
                    //Defines productions for members of a class

		            //MemberLst -> Member
                    new Production<string>("MemberLst".ToNonTerminal(), "Member".ToNonTerminal()),

                    //MemberLst -> MemberLst Member
                    new Production<string>("MemberLst".ToNonTerminal(), "MemberLst".ToNonTerminal(), "Member".ToNonTerminal()),

                    //Member -> Method
                    new Production<string>("Member".ToNonTerminal(), "Method".ToNonTerminal()),

                    //Member -> Field
                    new Production<string>("Member".ToNonTerminal(), "Field".ToNonTerminal()), 

                    //Member -> Property
                    new Production<string>("Member".ToNonTerminal(), "Property".ToNonTerminal()),
	                #endregion

                    #region Method
                    //Defines productions for method declarations

		            //MethodLst -> Method
                    new Production<string>("MethodLst".ToNonTerminal(), "Method".ToNonTerminal()),

                    //MethodLst -> MethodLst Method
                    new Production<string>("MethodLst".ToNonTerminal(), "MethodLst".ToNonTerminal(), "Method".ToNonTerminal()),

                    //Method -> MemberAccessMod ReturnType Id ( ) { StmtLst }
                    new Production<string>("Method".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "ReturnType".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), ")".ToTerminal(), "{".ToTerminal(), "StmtLst".ToNonTerminal(), "}".ToTerminal()),

                    //Method -> MemberAccessMod ReturnType Id ( ArgLst ) { StmtLst }
                    new Production<string>("Method".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "ReturnType".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), "ArgLst".ToNonTerminal(), ")".ToTerminal(), "{".ToTerminal(), "StmtLst".ToNonTerminal(), "}".ToTerminal()),

	                #endregion

                    #region Return Type
                    //Defines productions for a method return type.

		            //ReturnType -> TypeName | void
                    new Production<string>("ReturnType".ToNonTerminal(), "TypeName".ToNonTerminal()),
                    new Production<string>("ReturnType".ToNonTerminal(), "void".ToTerminal()), 
	                #endregion

                    #region Member Access Modifiers
		            //MemberAccessMod -> public | private | protected | internal
                    new Production<string>("MemberAccessMod".ToNonTerminal(), "public".ToTerminal()),
                    new Production<string>("MemberAccessMod".ToNonTerminal(), "private".ToTerminal()),
                    new Production<string>("MemberAccessMod".ToNonTerminal(), "protected".ToTerminal()),
                    new Production<string>("MemberAccessMod".ToNonTerminal(), "internal".ToTerminal()),

                    //MemberAccessMod -> proctected internal
                    new Production<string>("MemberAccessMod".ToNonTerminal(), "proctected".ToTerminal(), "internal".ToTerminal()),

                    //MemberAccessMod -> nothing
                    new Production<string>("MemberAccessMod".ToNonTerminal()), 
	                #endregion

                    #region Field
                    //Defines productions for a member field

                    //Matches "myField;", "myField = value", "public myField;", "public myField = value"
		            //Field -> MemberAccessMod Id AsgnExpr ;
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "Id".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()),

                    //Field -> MemberAccessMod bool|int|float|double|decimal|long|uint|ulong Expr ;
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "bool".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()),
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "int".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()),
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "float".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()),
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "double".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()),
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "decimal".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()), 
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "long".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()), 
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "uint".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()), 
                    new Production<string>("Field".ToNonTerminal(), "MemberAccessMod".ToNonTerminal(), "ulong".ToTerminal(), "FieldAsgnExpr".ToNonTerminal(), ";".ToTerminal()), 
	                #endregion

                    #region Property
                    //Defines productions that match to properties

		            //Property -> TypeName Id { PropGetExpr PropSetExpr }
                    new Production<string>("Property".ToNonTerminal(), "TypeName".ToNonTerminal(), "Id".ToTerminal(), "{".ToTerminal(), "PropGetExpr".ToNonTerminal(), "PropSetExpr".ToNonTerminal()),

                    //During semantic analysis 'Id' will be checked to make sure it is 'get'
                    //PropGetExpr -> Id ;
                    new Production<string>("PropGetExpr".ToNonTerminal(), "Id".ToTerminal(), ";".ToTerminal()),

                    //During semantic analysis 'Id' will be checked to make sure that it is 'get'
                    //PropGetExpr -> Id { StmtLst }
                    new Production<string>("PropGetExpr".ToNonTerminal(), "Id".ToTerminal(), "{".ToTerminal(), "StmtLst".ToNonTerminal(), "}".ToTerminal()),

                    //PropGetExpr -> nothing
                    new Production<string>("PropSetExpr".ToNonTerminal()),

                    //During semantic analysis 'Id' will be checked to make sure that it is 'set'
                    //PropSetExpr -> Id ;
                    new Production<string>("PropSetExpr".ToNonTerminal(), "Id".ToTerminal(), ";".ToTerminal()),

                    //During semantic analysis 'Id' will be checked to make sure that it is 'set'
                    //PropGetExpr -> Id { StmtLst }
                    new Production<string>("PropSetExpr".ToNonTerminal(), "Id".ToTerminal(), "{".ToTerminal(), "StmtLst".ToNonTerminal(), "}".ToTerminal()),

                    //PropSetExpr -> nothing
                    new Production<string>("PropSetExpr".ToNonTerminal()), 
	                #endregion

                    #region Field Assignment Expression
                    //Defines productions for field assignment(e.g. myField = stuff or just myField)

		            //FieldAsgnExpr -> AsgnExpr
                    new Production<string>("FieldAsgnExpr".ToNonTerminal(), "AsgnExpr".ToNonTerminal()),

                    //FieldAsgnExpr -> Id
                    new Production<string>("FieldAsgnExpr".ToNonTerminal(), "Id".ToTerminal()),  
	                #endregion

	                #endregion
                    
                    //AsgnExpr -> Id = Term
                    new Production<string>("AsgnExpr".ToNonTerminal(), "Id".ToTerminal(), "=".ToTerminal(), "Term".ToNonTerminal()),
                    
                    //StmtLst -> Stmt
                    new Production<string>("StmtLst".ToNonTerminal(), "Stmt".ToNonTerminal()),

                    //StmtLst -> Stmt StmtLst
                    new Production<string>("StmtLst".ToNonTerminal(), "Stmt".ToNonTerminal(), "StmtLst".ToNonTerminal()),

                    //Stmt -> Expr ;
                    new Production<string>("Stmt".ToNonTerminal(), "Expr".ToNonTerminal(), ";".ToTerminal()),

                    #region Expression
		            //Expr -> Term
                    new Production<string>("Expr".ToNonTerminal(), "Term".ToNonTerminal()),

                    //Expr -> Id = Term
                    new Production<string>("Expr".ToNonTerminal(), "Id".ToTerminal(), "=".ToTerminal(), "Term".ToNonTerminal()),

                    //Expr -> Expr BiOp Term
                    new Production<string>("Expr".ToNonTerminal(), "Expr".ToNonTerminal(), "BiOp".ToNonTerminal(), "Term".ToNonTerminal()), 
	                #endregion
                    

                    //BiOp -> +|-|*|/
                    new Production<string>("BiOp".ToNonTerminal(), "+".ToTerminal()),
                    new Production<string>("BiOp".ToNonTerminal(), "-".ToTerminal()),
                    new Production<string>("BiOp".ToNonTerminal(), "*".ToTerminal()),
                    new Production<string>("BiOp".ToNonTerminal(), "/".ToTerminal()),

                    //UnaryOp -> -
                    new Production<string>("UnaryOp".ToNonTerminal(), "-".ToTerminal()),

                    #region Term
		            //Term -> UnaryOp Term
                    new Production<string>("Term".ToNonTerminal(), "UnaryOp".ToNonTerminal(), "Term".ToNonTerminal()),

                    //Term -> ( Term )
                    new Production<string>("Term".ToNonTerminal(), "(".ToTerminal(), "Term".ToNonTerminal(), ")".ToTerminal()),

                    //Term -> null
                    new Production<string>("Term".ToNonTerminal(), "null".ToTerminal()),

                    //Term -> Id ( ParamLst )
                    new Production<string>("Term".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), "ParamLst".ToNonTerminal(), ")".ToTerminal()),

                    //Term -> Id ( )
                    new Production<string>("Term".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), ")".ToTerminal()),
                    
                    //Term -> Id
                    new Production<string>("Term".ToNonTerminal(), "Id".ToTerminal()), 
	                #endregion

                    //Defines an argument list(e.g. the signature of the method)

                    //ArgLst -> ArgLst, Arg
                    new Production<string>("ArgLst".ToNonTerminal(), "ArgLst".ToNonTerminal(), ",".ToTerminal(), "Arg".ToNonTerminal()),

                    //ArgLst -> nothing
                    //new Production<string>("ArgLst".ToNonTerminal()),

                    //ArgLst -> Arg
                    new Production<string>("ArgLst".ToNonTerminal(), "Arg".ToNonTerminal()),

                    //Arg -> Id Id
                    new Production<string>("Arg".ToNonTerminal(), "Id".ToTerminal(), "Id".ToTerminal()),

                    //ParamLst -> Param
                    new Production<string>("ParamLst".ToNonTerminal(), "Param".ToNonTerminal()),

                    //ParamLst -> ParamLst , Param
                    new Production<string>("ParamLst".ToNonTerminal(), "ParamLst".ToNonTerminal(), ",".ToTerminal(), "Param".ToNonTerminal()),

                    //Param -> Term
                    new Production<string>("Param".ToNonTerminal(), "Expr".ToNonTerminal())

                    #endregion
                }
            );
            #endregion

            #region Def
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //{
            //    Definitions = new ParserTokenDefinitionCollection<string>
            //    {
            //        Definitions = new List<ParserTokenDefinition<string>>
            //        {
            //            new StringedParserTokenDefinition(@"\w+", "id", true),
            //            new StringedParserTokenDefinition(@"->", "->", false),
            //            new StringedParserTokenDefinition(@";|\.", ";", false),
            //            new StringedParserTokenDefinition(@"\|", "|", false),
            //            new StringedParserTokenDefinition(@"\(", "(", false),
            //            new StringedParserTokenDefinition(@"\)", ")", false)
            //        }
            //    },
            //    Productions = new List<Production<string>>
            //    {
            //        //L -> P P
            //        new Production<string>("L".ToNonTerminal<string>(), "P".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //L -> P
            //        new Production<string>("L".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //P -> Term "->" Group ;
            //        new Production<string>("P".ToNonTerminal<string>(), "Term".ToNonTerminal<string>(), "->".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //P -> Term "->" Stmt ;
            //       // new Production<string>("P".ToNonTerminal<string>(), "Term".ToNonTerminal<string>(), "->".ToTerminal<string>(), "Stmt".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //Groups -> Group
            //        new Production<string>("Groups".ToNonTerminal<string>(), "Group".ToNonTerminal<string>()),

            //        //Groups -> Groups Group
            //        new Production<string>("Groups".ToNonTerminal<string>(), "Groups".ToNonTerminal<string>(), "Group".ToNonTerminal<string>()),

            //        //Group ->  Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Group -> Group | Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Group".ToNonTerminal<string>(), "|".ToTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Stmt -> ( Group )
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "(".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Stmt -> Terms
            //        new Production<string>("Stmt".ToNonTerminal<string>(false), "Terms".ToNonTerminal<string>()),

            //        //Terms ->  Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Terms -> Terms Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),



            //        //Stmt -> ( Stmt )
            //        //new Production<string>("Stmt".ToNonTerminal<string>(), "(".ToTerminal<string>(), "Stmt".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Stmt -> Stmt "|" Term
            //        //new Production<string>("Stmt".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>(), "|".ToTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Term -> id
            //        new Production<string>("Term".ToNonTerminal<string>(), "id".ToTerminal<string>())

            //    }
            //}; 
            #endregion

            #region Def
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //(
            //    new ParserTokenDefinitionCollection<string>
            //    (
            //        new List<ParserTokenDefinition<string>>
            //        {
            //            new StringedParserTokenDefinition(@"\w+", "id", true),
            //            new StringedParserTokenDefinition(@"->", "->", false),
            //            new StringedParserTokenDefinition(@";|\.", ";", false),
            //            new StringedParserTokenDefinition(@"\|", "|", false),
            //            new StringedParserTokenDefinition(@"\(", "(", false),
            //            new StringedParserTokenDefinition(@"\)", ")", false)
            //        }
            //    ),
            //    new List<Production<string>>
            //    {
            //        //L -> P P
            //        new Production<string>("L".ToNonTerminal<string>(), "L".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //L -> P
            //        new Production<string>("L".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //P -> Term "->" Group ;
            //        new Production<string>("P".ToNonTerminal<string>(), "Term".ToNonTerminal<string>(), "->".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //Group ->  Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Group -> Stmt | Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>(), "|".ToTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Stmt -> ( Group )
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "(".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Stmt ->  Terms
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "Terms".ToNonTerminal<string>()),

            //        //Terms -> Terms Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Terms -> Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Term -> id
            //        new Production<string>("Term".ToNonTerminal<string>(), "id".ToTerminal<string>())
            //    }
            //); 
            #endregion

            long totalParseTime = 0;
            long totalLexTime = 0;


            LRParser<Token<string>> parser = new LRParser<Token<string>>();
            parser.SetParseTable(def.GetGrammar());


            if (lexer == null)
            {
                lexer = new Lexer();
                lexer.SetDefintions(def.Definitions.GetNormalDefinitions());
            }
            Stopwatch w = Stopwatch.StartNew();


            tokens = lexer.ReadTokens(text);

            w.Stop();

            Stopwatch sw = Stopwatch.StartNew();

            ParseResult<Token<string>> tree = parser.ParseAST(def.ConvertToTerminals(tokens));

            sw.Stop();

            totalLexTime += w.ElapsedMilliseconds;
            totalParseTime += sw.ElapsedMilliseconds;

            MessageBox.Show(string.Format("Totally done. Average time to lex {0} chars: {1}. Average time to parse {2} tokens: {3}", text.Length, totalLexTime, tokens.Count(), totalParseTime), "Done", MessageBoxButtons.OK);
        }

        private void OnParseDone(dynamic param)
        {
            MessageBox.Show(string.Format("Parsing is done. {0} string chars were lexed in {1} milliseconds. {2} tokens were parsed in {3} milliseconds", param.l, param.w.ElapsedMilliseconds, param.p, param.sw.ElapsedMilliseconds), "Done", MessageBoxButtons.OK);
        }
    }
}
