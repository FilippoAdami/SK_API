«IMPORT MathLang»

«DEFINE generateClass FOR MathLang.Expression»
«EXPAND generateExpression(this)»
«ENDDEFINE»

«DEFINE generateExpression FOR MathLang.AdditionExpression»
public class AdditionExpression {
    public int evaluate() {
        return «EXPAND generateExpression(left)» + «EXPAND generateExpression(right)»;
    }
}
«ENDDEFINE»

«DEFINE generateExpression FOR MathLang.SubtractionExpression»
public class SubtractionExpression {
    public int evaluate() {
        return «EXPAND generateExpression(left)» - «EXPAND generateExpression(right)»;
    }
}
«ENDDEFINE»

«DEFINE generateExpression FOR MathLang.MultiplicationExpression»
public class MultiplicationExpression {
    public int evaluate() {
        return «EXPAND generateExpression(left)» * «EXPAND generateExpression(right)»;
    }
}
«ENDDEFINE»

«DEFINE generateExpression FOR MathLang.DivisionExpression»
public class DivisionExpression {
    public int evaluate() {
        return «EXPAND generateExpression(left)» / «EXPAND generateExpression(right)»;
    }
}
«ENDDEFINE»

«DEFINE generateExpression FOR MathLang.NumberExpression»
public class NumberExpression {
    private int value;

    public NumberExpression(int value) {
        this.value = value;
    }

    public int evaluate() {
        return value;
    }
}
«ENDDEFINE»

«DEFINE generateExpression FOR MathLang.GroupExpression»
public class GroupExpression {
    private MathLang.Expression expression;

    public GroupExpression(MathLang.Expression expression) {
        this.expression = expression;
    }

    public int evaluate() {
        return «EXPAND generateExpression(expression)»;
    }
}
«ENDDEFINE»
