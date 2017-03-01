using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoCore.AST.AssociativeAST;
using ImperativeNode = ProtoCore.AST.ImperativeAST.ImperativeNode;
using ProtoCore.AST;
using System.Collections;

namespace ProtoCore.SyntaxAnalysis
{
    class AstEnumerable : AstVisitor<int, int>, IEnumerable<Node>
    {
        private List<Node> nodes = new List<Node>();
        
        public AstEnumerable(Node node)
        {
            VisitNode(node);
        }

        public AstEnumerable(IEnumerable<AssociativeNode> nodes)
        {
            VisitNodes(nodes);
        }

        public AstEnumerable(IEnumerable<ImperativeNode> nodes)
        {
            VisitNodes(nodes);
        }

        public override int VisitCommentNode(CommentNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitLanguageBlockNode(LanguageBlockNode node)
        {
            return VisitAssociativeNode(node) + VisitNode(node.CodeBlockNode);
        }

        private int VisitNode(Node node)
        {
            var anode = node as AssociativeNode;
            if (anode != null) return anode.Accept(this);

            var inode = node as ImperativeNode;
            return (inode != null) ? inode.Accept(this) : 0;
        }

        public override int VisitReplicationGuideNode(ReplicationGuideNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitAtLevelNode(AtLevelNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitArrayNameNode(ArrayNameNode node)
        {
            var n = VisitAssociativeNode(node); //visit current node

            //Traverse replication guides
            n += VisitNodes(node.ReplicationGuides);

            //Traverse array dimensions if any
            if(node.ArrayDimensions != null)
            {
                n += node.ArrayDimensions.Accept(this);
            }
            //Traverse @Level node if present.
            return (node.AtLevel != null) ? n + node.AtLevel.Accept(this) : n;
        }

        public override int VisitGroupExpressionNode(GroupExpressionNode node)
        {
            //GroupExpression is an ArrayName node, so visit array name node first
            //and then traverse its expression.
            return VisitArrayNameNode(node) + node.Expression.Accept(this);
        }

        public override int VisitIdentifierNode(IdentifierNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitTypedIdentifierNode(TypedIdentifierNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitIdentifierListNode(IdentifierListNode node)
        {
            //Traverse left and then right nodes
            return node.LeftNode.Accept(this) + node.RightNode.Accept(this);
        }

        public override int VisitIntNode(IntNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitDoubleNode(DoubleNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitBooleanNode(BooleanNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitCharNode(CharNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitStringNode(StringNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitNullNode(NullNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitFunctionCallNode(FunctionCallNode node)
        {
            //Traverse this node as ArrayNameNode and then traverse function
            return VisitArrayNameNode(node) + node.Function.Accept(this);
        }

        public override int VisitFunctionDotCallNode(FunctionDotCallNode node)
        {
            return VisitAssociativeNode(node) + node.FunctionCall.Accept(this);
        }

        public override int VisitVarDeclNode(VarDeclNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitArgumentSignatureNode(ArgumentSignatureNode node)
        {
            return VisitAssociativeNode(node) + VisitNodes(node.Arguments);
        }

        public override int VisitCodeBlockNode(CodeBlockNode node)
        {
            return VisitAssociativeNode(node) + VisitNodes(node.Body);
        }

        public override int VisitClassDeclNode(ClassDeclNode node)
        {
            return VisitAssociativeNode(node) + VisitNodes(node.Procedures) + VisitNodes(node.Variables);
        }

        public override int VisitConstructorDefinitionNode(ConstructorDefinitionNode node)
        {
            var n = VisitAssociativeNode(node);
            return (node.FunctionBody != null) ? node.FunctionBody.Accept(this) + n : n;
        }

        public override int VisitFunctionDefinitionNode(FunctionDefinitionNode node)
        {
            var n = VisitAssociativeNode(node);
            return (node.FunctionBody != null) ? node.FunctionBody.Accept(this) + n : n;
        }

        public override int VisitIfStatementNode(IfStatementNode node)
        {
            var n = VisitAssociativeNode(node);
            n += node.ifExprNode.Accept(this);
            return n + VisitNodes(node.IfBody) + VisitNodes(node.ElseBody);
        }

        public override int VisitInlineConditionalNode(InlineConditionalNode node)
        {
            var n = VisitAssociativeNode(node);
            n += (node.ConditionExpression != null) ? node.ConditionExpression.Accept(this) : 0;
            n += (node.TrueExpression != null) ? node.TrueExpression.Accept(this) : 0;
            n += (node.FalseExpression != null) ? node.FalseExpression.Accept(this) : 0;
            return n; 
        }

        public override int VisitBinaryExpressionNode(BinaryExpressionNode node)
        {
            return VisitAssociativeNode(node) + node.LeftNode.Accept(this) + node.RightNode.Accept(this);
        }

        public override int VisitUnaryExpressionNode(UnaryExpressionNode node)
        {
            return VisitAssociativeNode(node) + node.Expression.Accept(this);
        }

        public override int VisitRangeExprNode(RangeExprNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitExprListNode(ExprListNode node)
        {
            return VisitArrayNameNode(node) + VisitNodes(node.Exprs);
        }

        public override int VisitArrayNode(ArrayNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitImportNode(ImportNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitDynamicNode(DynamicNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitDynamicBlockNode(DynamicBlockNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitThisPointerNode(ThisPointerNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitDefaultArgNode(DefaultArgNode node)
        {
            return VisitAssociativeNode(node);
        }

        public override int VisitLanguageBlockNode(AST.ImperativeAST.LanguageBlockNode node)
        {
            return VisitImperativeNode(node) + VisitNode(node.CodeBlockNode);
        }

        public override int VisitArrayNameNode(AST.ImperativeAST.ArrayNameNode node)
        {
            var n =  VisitImperativeNode(node);
            return (node.ArrayDimensions != null) ? node.ArrayDimensions.Accept(this) + n : n;
        }

        public override int VisitGroupExpressionNode(AST.ImperativeAST.GroupExpressionNode node)
        {
            //GroupExpression is an ArrayName node, so visit array name node first
            //and then traverse its expression.
            return VisitArrayNameNode(node) + node.Expression.Accept(this);
        }

        public override int VisitIdentifierNode(AST.ImperativeAST.IdentifierNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitTypedIdentifierNode(AST.ImperativeAST.TypedIdentifierNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitIdentifierListNode(AST.ImperativeAST.IdentifierListNode node)
        {
            //Traverse left and then right nodes
            return node.LeftNode.Accept(this) + node.RightNode.Accept(this);
        }

        public override int VisitIntNode(AST.ImperativeAST.IntNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitDoubleNode(AST.ImperativeAST.DoubleNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitBooleanNode(AST.ImperativeAST.BooleanNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitCharNode(AST.ImperativeAST.CharNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitStringNode(AST.ImperativeAST.StringNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitNullNode(AST.ImperativeAST.NullNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitFunctionCallNode(AST.ImperativeAST.FunctionCallNode node)
        {
            //Traverse this node as ArrayNameNode and then traverse function
            return VisitArrayNameNode(node) + node.Function.Accept(this);
        }

        public override int VisitCodeBlockNode(AST.ImperativeAST.CodeBlockNode node)
        {
            return VisitImperativeNode(node) + VisitNodes(node.Body);
        }

        public override int VisitIfStatementNode(AST.ImperativeAST.IfStmtNode node)
        {
            var n = VisitImperativeNode(node);
            n += node.IfExprNode.Accept(this);
            return n + VisitNodes(node.ElseIfList) + VisitNodes(node.ElseBody);
        }

        public override int VisitElseIfNode(AST.ImperativeAST.ElseIfBlock node)
        {
            return VisitImperativeNode(node) + VisitNodes(node.Body);
        }

        public override int VisitInlineConditionalNode(AST.ImperativeAST.InlineConditionalNode node)
        {
            var n = VisitImperativeNode(node);
            n += (node.ConditionExpression != null) ? node.ConditionExpression.Accept(this) : 0;
            n += (node.TrueExpression != null) ? node.TrueExpression.Accept(this) : 0;
            n += (node.FalseExpression != null) ? node.FalseExpression.Accept(this) : 0;
            return n;
        }

        public override int VisitBinaryExpressionNode(AST.ImperativeAST.BinaryExpressionNode node)
        {
            return VisitImperativeNode(node) + node.LeftNode.Accept(this) + node.RightNode.Accept(this);
        }

        public override int VisitUnaryExpressionNode(AST.ImperativeAST.UnaryExpressionNode node)
        {
            return VisitImperativeNode(node) + node.Expression.Accept(this);
        }

        public override int VisitRangeExprNode(AST.ImperativeAST.RangeExprNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitExprListNode(AST.ImperativeAST.ExprListNode node)
        {
            return VisitArrayNameNode(node) + VisitNodes(node.Exprs);
        }

        public override int VisitArrayNode(AST.ImperativeAST.ArrayNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitWhileStatementNode(AST.ImperativeAST.WhileStmtNode node)
        {
            return VisitImperativeNode(node) + node.Expr.Accept(this) + VisitNodes(node.Body);
        }

        public override int VisitForLoopNode(AST.ImperativeAST.ForLoopNode node)
        {
            return VisitImperativeNode(node) + node.Expression.Accept(this) + VisitNodes(node.Body);
        }
        public override int VisitBreakNode(AST.ImperativeAST.BreakNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitContinueNode(AST.ImperativeAST.ContinueNode node)
        {
            return VisitImperativeNode(node);
        }

        public override int VisitIfStmtPositionNode(AST.ImperativeAST.IfStmtPositionNode node)
        {
            return VisitImperativeNode(node);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        private int VisitNodes(IEnumerable<AssociativeNode> nodes)
        {
            return nodes.Aggregate(0, (x, n) => x + n.Accept(this));
        }

        private int VisitNodes(IEnumerable<ImperativeNode> nodes)
        {
            return nodes.Aggregate(0, (x, n) => x + n.Accept(this));
        }

        public override int VisitAssociativeNode(AssociativeNode node)
        {
            nodes.Add(node);
            return 1;
        }

        public override int VisitImperativeNode(ImperativeNode node)
        {
            nodes.Add(node);
            return 1;            
        }
    }

    public static class Utils
    {
        /// <summary>
        /// Return a list of all children recursively including this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Node> EnumerateAll(this Node node)
        {
            return new AstEnumerable(node);
        }

        /// <summary>
        /// Return a list of all children recursively by traversing all associative nodes.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static IEnumerable<Node> EnumerateAll(this IEnumerable<AssociativeNode> nodes)
        {
            return new AstEnumerable(nodes);
        }


        /// <summary>
        /// Return a list of all children recursively by traversing all imperative nodes.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static IEnumerable<Node> EnumerateAll(this IEnumerable<ImperativeNode> nodes)
        {
            return new AstEnumerable(nodes);
        }
    }
}
