# Semantic Kernel powered Educational APIs

This repository contains a set of APIs designed to utilize the Semantic Kernel. The APIs are restricted to authorized users with valid tokens.

## APIs Overview

### MaterialAnalyser API

This API determines the language, the macro subject, the title, the level of difficulty (such as middle school, college etc.) and the list of topics explained in a text educational resource.

### Learning Objective Generator API

This API generates a list of learning objectives for each Bloom's Taxonomy level given a topic and a level (such as middle school, college etc.)

### Learning Objective Analyser API

This API determines the Bloom's Taxonomy level associated with a given learning objective. It provides responses such as Remembering, Understanding, Applying, Analyzing, Evaluating, or Creating.

### Exercise Generation API

The Exercise Generation API assists in creating exercises based on provided examples. It generates exercises aligned with Bloom's Taxonomy levels and specific learning objectives.

### Exercise Corrector API

The Exercise Corrector API assists in correcting open ended exercises based on provided examples. It generates an evaluation of the answer and explains possible corrections.

## Usage

To utilize these APIs, authorized users must possess a valid token. The APIs provide valuable insights into text analysis, learning objective assessment, and exercise generation.

## API Usage Examples
!The following JSON are the lists of the context variables, they do not correspond to the actual input models

- **Material Analyser API**:
```json
{
  "examples": "{{$examples}}",
  "material": "{{$material}}"
}
```
- **Learning Objective Generator API**:
```json
{
  "bloom_level": "{{$bloom_level}}"
  "level_of_difficulty":"{{$level}}"
  "topic": "{{$topic}}"
}
```
- **Learning Objective Analyser API**:
```json
{
  "examples": "{{$examples}}",
  "learning_objective": "{{$learning_objective}}"
}
```
- **Exercise Generator API**:
```json
{
  "examples": "{{$examples}}",
  "format": "{{$format}}",
  "difficulty_level": "{{$difficulty_level}}",
  "domain_of_expertise": "{{$domain_of_expertise}}",
  "material": "{{$material}}",
  "type_of_exercise": "{{$type_of_exercise}}",
  "learning_objective": "{{$learning_objective}}",
  "A_format": "{{$A_format}}",
  "type_of_assignment": "{{$type_of_assignment}}",
  "bloom_level": "{{$bloom_level}}",
  "topic": "{{$topic}}",
  "number_of_solutions": "{{$number_of_solutions}}",
  "type_of_solution": "{{$type_of_solution}}",
  "description": "{{$description}}",
  "S_format": "{{$S_format}}"
}
```
- **Exercise Corrector API**:
```json
{
  "correct_answer": "{{$correct_answer}}"
  "question": "{{$question}}"
  "given_answer": "{{$given_answer}}"
}
```

## API Prompts

- **Material Analyser API**:
```
  Please write everything in your answer in english and organize the outputs in the following format (correct formatting is crucial for post-processing):
    0.) Language: [Extracted Language]
    1.) Macro-Subject: [Extracted Macro-Subject]
    2.) Title: [Generated Title]
    3.) Perceived Level of Difficulty: [Difficulty Rating/Description]
    4.) Main Topics and Explanations:
        1.) Topic: [Topic 1], Type: [Type 1], Description: [Description 1]
        2.) Topic: [Topic 2], Type: [Type 2], Description: [Description 2]
        ...
        N.) Topic: [Topic N], Type: [Type N], Description: [Description N]
            
  Examples:
  {{$examples}}   
  
  Given the provided material: '{{$material}}', please analyze it and generate the following outputs:

  0.) Language:
  Try to understand the language in which the material is written.
  1.) Macro-Subject: 
  Extract the macro-subject of the material, such as history, math, literature, etc..
  2.) Title: 
  Generate a title that best summarizes the material and reflects its main focus and content.
  3.) Perceived Level of Difficulty: 
  Assess the perceived level of difficulty of the material and provide a rating or description indicating its complexity.The perceived level should fit one of the following categories: primary_school, middle_school, high_school, college, or academy.
  4.)Main Topics and Explanations:
  Extract all the N main micro-topics covered in the material, provide a list of tuples containing:
  Topic: topic, Type: type, Description: description

  Ensure the topics are ordered by their appearance in the material and provide concise, two-line descriptions of how each topic is explained. 
  The type must be either 'theoretical', 'code' or 'problem_resolution', where 'code' has to be returned for all the topics that talk about programming,  'problem_resolution' for all scientific topics and 'theoretical' has to be returned only if neither 'code' nor 'problem_resolution' are applicable.
```
- **Learning Objective Generator API**:
```
  You are an {{$difficulty_level}} level {{$topic}} teacher. You want to program your next lesson. To do so, you need to define exactly 6 concrete, specific and very descriptive learning objectives, one for each Bloom's Taxonomy level.
  The output must be in the format:
  Remembering: level remembering learning objective
  Understanding: level understanding learning objective
  Applying: applying level learning objective
  Analyzing: analyziong level learning objective
  Evaluating: evaluating level learning objective
  Creating: creating level learning objective
```
- **Learning Objective Analyser API**:
```
  Given a learning objective, determine the Bloom's Taxonomy level associated with it. Please specify the Bloom's level as one of the following: Remembering, Understanding, Applying, Analyzing, Evaluating, or Creating.

  Learning Objective: {{$learning_objective}}

  Bloom's Taxonomy Levels:
  0. None: The Learning objective is not valid.
  1. Remembering: Recalling or recognizing information.
  2. Understanding: Demonstrating comprehension of concepts or ideas.
  3. Applying: Using knowledge or understanding in new situations.
  4. Analyzing: Breaking down information into parts and examining relationships.
  5. Evaluating: Making judgments based on criteria and evidence.
  6. Creating: Generating new ideas, products, or solutions.

  Examples:
  {{$examples}}

  ---

  Instructions for GPT:
  1. Read the provided learning objective.
  2. Determine the primary cognitive process or skill required by the objective.
  3. Select the appropriate Bloom's Taxonomy level based on the cognitive process.
  4. Provide the Bloom's level as your response. (The output must be only the Bloom's level, it's crucial for further processing.)
```
- **Exercise Generator API**:
  The following JSON is the list of the context variables, it does not correspond to the actual input model
```
  Hello! Here are some approved examples of valid outputs for your reference. Please utilize these examples to guide the generation of your final answer, ensuring consistency and quality. Examples: 
  {{$examples}}
  Here's the skeleton structure for formatting the output. Please use this format to organize your final answer consistently. Format: 
  {{$format}}
  You are a {{$difficulty_level}} level {{$domain_of_expertise}} professor who just gave a lecture on this material: 
  {{$material}}.
  Now, your objective is to assess the level of comprehension of your students about your last lesson. Drawing from your {{$domain_of_expertise}} expertise, your aim is to craft one {{$type_of_exercise}} exercise that aims to {{$learning_objective}}.
  
  Below is the skeleton structure for formatting the assignment. Please adhere to this format to ensure consistency in your final answer. Format: 
  {{$A_format}}
  Now, generate a {{$type_of_assignment}}{{$type_of_exercise}} for {{$difficulty_level}} level {{$domain_of_expertise}} students. Ensure that the exercise aligns with '{{$bloom_level}}' Bloom's taxonomy level and pertains to the topic of {{$topic}}.
  Please design the assignment to include {{$number_of_solutions}}{{$type_of_solution}} potential correct solution/s and consider that the assignment should be clear on the instructions and that {{$description}}. Present the final assignment in {{$type_of_assignment}} format.

  Below is the skeleton structure for formatting the solution for the assignment. Please adhere to this format to ensure consistency in your final answer. Format: 
  {{$S_format}}
  Now you need to define {{$number_of_solutions}} correct solution/s as {{$type_of_solution}} for the assignment. To generate the solution/s you need to {{$indications}}
  Generate {{$number_of_solutions}} distractors for each solution, designed to challenge students by closely resembling the correct solution, while maintaining similarity in style and format.
  Now generate {{$number_of_solutions}} easily discardable distractor for each solution, clearly distinguishable as incorrect, while maintaining similarity in style and format to the correct solution.        
  Now output your final response in the format provided at the beginning.
```
- **Exercise Corrector API**:
```
  You are a teacher and you need to evaluate a test. Given the following question and answer, you need to evaluate the accuracy of the answer and, eventually, correct it.

  The output should fit the format:
  Accuracy: from 0 (if completely wrong) to 1 (if completely correct) with 0.2 intervals
  Correct answer: null if accuracy < 0.8
  What was wrong in the answer and why: null if accuracy <0.8
  
  Here are some examples:
  {{$examples}}

  Question: {{$question}}
  Correct Answer: {{$correct_answer}}
  Answer: {{$answer}}
  Accuracy:
  Correct answer:
  What was wrong and why:
```

## Example of a valid material: 
```
Language Declaration
Each Xtext grammar starts with a header that defines some properties of the
grammar.  
    grammar org.example.domainmodel.Domainmodel
            with org.eclipse.xtext.common.Terminals 
The first line declares the name of the language. Xtext leverages Java’s class
path mechanism. This means that the name can be any valid Java qualifier. The
grammar file name needs to correspond to the language name and have the file
extension `.xtext`. This means that the name has to be e.g.
_Domainmodel.xtext_ and must be placed in a package _org.example.domainmodel_
on your project’s class path. In other words, your `.xtext` file has to reside
in a Java source folder to be valid.
The second aspect that can be deduced from the first line of the grammar is
its relationship to other languages. An Xtext grammar can declare another
existing grammar to be reused. The mechanism is called [grammar
mixin](301_grammarlanguage.html#grammar-mixins).

Code Generation
Once you have a language you probably want to do something with it. There are
two options, you can either write an interpreter that inspects the AST and
does something based on that or you translate your language to another
programming language or configuration files. In this section we’re going to
show how to implement a code generator for an Xtext-based language.
### IGenerator2
If you go with the default MWE workflow for your language and you haven’t used
Xbase, you are provided with a callback stub that implements
[IGenerator2](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/generator/IGenerator2.java)
by extending the
[AbstractGenerator](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/generator/AbstractGenerator.java)
base class. It has three methods that are called from the builder
infrastructure whenever a DSL file has changed or should be translated
otherwise. The three parameters passed in to those method are:
  * The resource to be processed
  * An instance of [IFileSystemAccess2](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/generator/IFileSystemAccess2.java)
  * An instance of [IGeneratorContext](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/generator/IGeneratorContext.java)
The [IFileSystemAccess2](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/generator/IFileSystemAccess2.java)
API abstracts over the different file systems the code generator may run over.
When the code generator is triggered within the incremental build
infrastructure in Eclipse the underlying file system is the one provided by
Eclipse, and when the code generator is executed outside Eclipse, say in a
headless build, it is `java.io.File`.
A very simple implementation of a code generator for the [state machine
example](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext.xtext.ui.examples/projects/fowlerdsl/org.eclipse.xtext.example.fowlerdsl/src/org/eclipse/xtext/example/fowlerdsl/Statemachine.xtext)
could be the following:    
    class StatemachineGenerator extends AbstractGenerator {  
        override doGenerate(Resource resource, IFileSystemAccess2 fsa, IGeneratorContext context) {
            fsa.generateFile("relative/path/AllTheStates.txt", '''
                «FOR state : resource.allContents.filter(State).toIterable»
                    State «state.name»
                «ENDFOR»
            ''')
        }    
    }    
### Output Configurations
You don’t want to deal with platform or even installation dependent paths in
your code generator, rather you want to be able to configure the code
generator with some basic outlet roots where the different generated files
should be placed under. This is what output configurations are made for.
By default every language will have a single outlet, which points to
`<project-root>/src-gen/`. The files that go here are treated as fully derived
and will be erased by the compiler automatically when a new file should be
generated. If you need additional outlets or want to have a different default
configuration, you need to implement the interface
[IOutputConfigurationProvider](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/generator/IOutputConfigurationProvider.java).
It’s straightforward to understand and the default implementation gives you a
good idea about how to implement it.
With this implementation you lay out the basic defaults which can be changed
by users on a workspace or per project level using the preferences.

Validation
Static analysis is one of the most interesting aspects when developing a
programming language. The users of your languages will be grateful if they get
informative feedback as they type. In Xtext there are basically three
different kinds of validation.
### Automatic Validation
Some implementation aspects (e.g. the grammar, scoping) of a language have an
impact on what is required for a document or semantic model to be valid. Xtext
automatically takes care of this.
#### Lexer/Parser: Syntactical Validation
The syntactical correctness of any textual input is validated automatically by
the parser. The error messages are generated by the underlying parser
technology. One can use the
[ISyntaxErrorMessageProvider](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/parser/antlr/ISyntaxErrorMessageProvider.java)
API to customize these messages. Any syntax errors can be retrieved from the
Resource using the common EMF API: the
[`Resource.getErrors()`](https://git.eclipse.org/r/plugins/gitiles/emf/org.eclipse.emf/+/refs/tags/R2_20_0/plugins/org.eclipse.emf.ecore/src/org/eclipse/emf/ecore/resource/Resource.java)
and
[`Resource.getWarnings()`](https://git.eclipse.org/r/plugins/gitiles/emf/org.eclipse.emf/+/refs/tags/R2_20_0/plugins/org.eclipse.emf.ecore/src/org/eclipse/emf/ecore/resource/Resource.java)
method invocations.
#### Linker: Cross-reference Validation
Any broken cross-references can be checked generically. As cross-reference
resolution is done lazily (see linking), any broken links are resolved lazily
as well. If you want to validate whether all links are valid, you will have to
navigate through the model so that all installed EMF proxies get resolved.
This is done automatically in the editor.
Similarly to syntax errors, any unresolvable cross-links will be reported and
can be obtained through the
[`Resource.getErrors()`](https://git.eclipse.org/r/plugins/gitiles/emf/org.eclipse.emf/+/refs/tags/R2_20_0/plugins/org.eclipse.emf.ecore/src/org/eclipse/emf/ecore/resource/Resource.java)
and
[`Resource.getWarnings()`](https://git.eclipse.org/r/plugins/gitiles/emf/org.eclipse.emf/+/refs/tags/R2_20_0/plugins/org.eclipse.emf.ecore/src/org/eclipse/emf/ecore/resource/Resource.java)
method invocations.
#### Serializer: Concrete Syntax Validation
The
[IConcreteSyntaxValidator](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/IConcreteSyntaxValidator.java)
validates all constraints that are implied by a grammar. Meeting these
constraints is mandatory for a model to be serialized.
Example:     
    MyRule:
        ({MySubRule} "sub")? (strVal+=ID intVal+=INT)*;
This implies several constraints:
  1. Types: only instances of _MyRule_ and _MySubRule_ are allowed for this rule. Subtypes are prohibited, since the parser never instantiates unknown subtypes.
  2. Features: In case the _MyRule_ and _MySubRule_ have [EStructuralFeatures](https://git.eclipse.org/r/plugins/gitiles/emf/org.eclipse.emf/+/refs/tags/R2_20_0/plugins/org.eclipse.emf.ecore/src/org/eclipse/emf/ecore/EStructuralFeature.java) besides _strVal_ and _intVal_ , only _strVal_ and _intVal_ may have non-transient values.
  3. Quantities: The following condition must be true: `strVal.size() == intVal.size()`.
  4. Values: It must be possible to convert all values to valid tokens for the used terminal rules _ID_ and _INT_.
The typical use case for the concrete syntax validator is validation in non-
Xtext-editors that use an
[XtextResource](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/resource/XtextResource.java).
This is the case when combining GMF and Xtext, for example. Another use case
is when the semantic model is modified “manually” (not by the parser) and then
serialized again. Since it is very difficult for the serializer to provide
meaningful error messages, the concrete syntax validator is executed by
default before serialization. A textual Xtext editor itself is _not_ a valid
use case. Here, the parser ensures that all syntactical constraints are met.
Therefore there is no value in additionally running the concrete syntax
validator.
There are some limitations to the concrete syntax validator which result from
the fact that it treats the grammar as declarative, which is something the
parser doesn’t always do.
  * Grammar rules containing assigned actions (e.g. `{MyType.myFeature=current}`) are ignored. Unassigned actions (e.g. `{MyType}`), however, are supported.
  * Grammar rules that delegate to one or more rules containing assigned actions via unassigned rule calls are ignored.
  * Orders within list-features cannot be validated. e.g. `Rule: (foo+=R1 foo+=R2)*` implies that _foo_ is expected to contain instances of _R1_ and _R2_ in an alternating order.
To use concrete syntax validation you can let Guice inject an instance of
[IConcreteSyntaxValidator](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/IConcreteSyntaxValidator.java)
and use it directly. Furthermore, there is an
[adapter](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/impl/ConcreteSyntaxEValidator.java)
which allows to use the concrete syntax validator as an
[EValidator](https://git.eclipse.org/r/plugins/gitiles/emf/org.eclipse.emf/+/refs/tags/R2_20_0/plugins/org.eclipse.emf.ecore/src/org/eclipse/emf/ecore/EValidator.java).
You can, for example, enable it in your runtime module, by adding:    
    @SingletonBinding(eager = true)
    public Class<? extends ConcreteSyntaxEValidator> bindConcreteSyntaxEValidator() {
        return ConcreteSyntaxEValidator.class;
    }
To customize the error messages please see
[IConcreteSyntaxDiagnosticProvider](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/IConcreteSyntaxDiagnosticProvider.java)
and subclass
[ConcreteSyntaxDiagnosticProvider](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/impl/ConcreteSyntaxDiagnosticProvider.java).
### Custom Validation
In addition to the afore mentioned kinds of validation, which are more or less
done automatically, you can specify additional constraints specific for your
Ecore model. The Xtext language generator will provide you two Java classes.
The first is an abstract class generated to _src-gen/_ which extends the
library class
[AbstractDeclarativeValidator](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/AbstractDeclarativeValidator.java).
This one just registers the EPackages for which this validator introduces
constraints. The other class is a subclass of that abstract class and is
generated to the _src/_ folder in order to be edited by you. That is where you
put the constraints in.
The purpose of the
[AbstractDeclarativeValidator](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/AbstractDeclarativeValidator.java)
is to allow you to write constraints in a declarative way - as the class name
already suggests. That is instead of writing exhaustive if-else constructs or
extending the generated EMF switch you just have to add the
[Check](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/Check.java)
annotation to any method and it will be invoked automatically when validation
takes place. Moreover you can state for what type the respective constraint
method is, just by declaring a typed parameter. This also lets you avoid any
type casts. In addition to the reflective invocation of validation methods the
AbstractDeclarativeValidator provides a few convenient assertions.
The Check annotation has a parameter that can be used to declare when a check
should run: _FAST_ will run whenever a file is modified, _NORMAL_ checks will
run when saving the file, and _EXPENSIVE_ checks will run when explicitly
validating the file via the menu option. Here is an example written in Java: 
    public class DomainmodelValidator extends AbstractDomainmodelValidator { 
        @Check
        public void checkNameStartsWithCapital(Entity entity) {
            if (!Character.isUpperCase(entity.getName().charAt(0))) {
                warning("Name should start with a capital", 
                    DomainmodelPackage.Literals.TYPE__NAME);
            }
        }
    }
You can use the
[IResourceValidator](https://github.com/eclipse/xtext/blob/main/org.eclipse.xtext/src/org/eclipse/xtext/validation/IResourceValidator.java)
to validate a given resource programmatically. Example: 
    @Inject IResourceValidator resourceValidator;    
    public void checkResource(Resource resource) {
        List<Issue> issues = resourceValidator.validate(resource,
                CheckMode.ALL, CancelIndicator.NullImpl);
        for (Issue issue: issues) {
            switch (issue.getSeverity()) {
                case ERROR:
                    System.out.println("ERROR: " + issue.getMessage());
                    break;
                case WARNING:
                    System.out.println("WARNING: " + issue.getMessage());
                    break;
                default: // do nothing
            }
        }
    }
You can also implement quick fixes for individual validation errors and
warnings. See the [section on quick fixes](310_eclipse_support.html#quick-
fixes) for details.
```
