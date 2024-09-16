namespace SK_API
{
    public class TextAnalyserPrompt {
        public static string TextAnalysisPrompt = @"
Please write **everything** in your answer in English and organize the outputs in the following format (correct formatting is crucial for post-processing):{{$format}}
Valid Examples: {{$examples}}
Material: '{{$material}}'. Analyze it and generate the output in the correct format like the valid examples.
Ensure the topics are ordered by their appearance in the material and provide appropriate titles and concise, two-line descriptions (in English) of how each topic is explained.
The type must be either 'theoretical', 'code' or 'problem_resolution', where 'code' has to be returned for all the topics that talk specifically about programming,  'problem_resolution' for all scientific topics and 'theoretical' has to be returned only if neither 'code' nor 'problem_resolution' are applicable.
The Bloom's level must be one of the following: ""Remembering"", ""Understanding"", ""Applying"", ""Analyzing"", ""Evaluating"", ""Creating"".
The starting words MUST match exactly the beginning of the topic section in the material, it's crucial for the post-processing segmentation of the text.
Provide **ONLY** the JSON."; 
    }
    public class ActivityGenerationPrompt {
        public static string Examples(string examples) {
            return @$"Hello! Here are some approved examples of valid outputs for your reference. Please utilize these examples to guide the generation of your final answer, ensuring consistency and quality. Examples: {examples}";
        }
        public static string Format(string format){
            return @$"Here's the skeleton structure for formatting the output. Please use this format to organize your final answer consistently. Format: {format}";
        }
        public static string Personification(){
            return @"You are a {{$difficulty_level}} level {{$domain_of_expertise}} professor who just gave a lecture on {{$lesson_title}}. Here's the material of the lesson you just provided: {{$material}}.
Now, your objective is to assess the level of comprehension of your Learners about your last lesson. Drawing from your {{$domain_of_expertise}} expertise, your aim is to craft one {{$type_of_exercise}} exercise with related learning objective of '{{$learning_objective}}'.";
        }
        
        public static string A_Description(){
            return @"Now, generate a {{$type_of_assignment}} {{$type_of_exercise}} assignment for {{$difficulty_level}} level {{$domain_of_expertise}} Learners. Ensure that the exercise aligns with '{{$bloom_level}}' Bloom's taxonomy level, pertains to the topic of '{{$topic}}' and is consistent with the information provided in the material of the lesson.";
        }
        public static string A_Resolution(string description, string type_of_solution, string number_of_solutions) {
            return @$"Please consider that the assignment request must be designed to allow {number_of_solutions} {type_of_solution} correct solution/solutions. The assignment should be clear on the instructions and {description}.";
        }

        public static string S_Solution(string type_of_solution, string indications, string number_of_solutions){
            return @$"Now you need to define {number_of_solutions} {type_of_solution} correct solution/solutions for the assignment. To generate the solution/solutions you need to {indications}";
        }
        public static string S_Distractors(){
            return @"Generate {{$number_of_distractors}} distractors for each solution, designed to challenge Learners by closely resembling the correct solution, while maintaining similarity in style and format.";
        }
        public static string S_EasilyDiscardableDistractors(){
            return "Now generate {{$number_of_easily_discardable_distractors}} easily discardable distractor for each solution, clearly distinguishable as incorrect, while maintaining similarity in style and format to the correct solution.";
        }
        
        public static string Ending(){
            return "Now output your final response in the format provided at the beginning, just like the provided examples.";
        }
    }
    public class LearningObjectivePrompts{
        public static string LearningObjectiveAnalysis = @"Given a learning objective, determine the Bloom's Taxonomy level , the macro subject, the level and the topic associated with it. Please specify the Bloom's level as one of the following: Remembering, Understanding, Applying, Analyzing, Evaluating, or Creating.

            Learning Objective: {{$learningObjective}}

            Bloom's Taxonomy Levels:
            0. Remembering: Recalling or recognizing information.
            1. Understanding: Demonstrating comprehension of concepts or ideas.
            2. Applying: Using knowledge or understanding in new situations.
            3. Analyzing: Breaking down information into parts and examining relationships.
            4. Evaluating: Making judgments based on criteria and evidence.
            5. Creating: Generating new ideas, products, or solutions.

            Levels:
            0. primary_school
            1. middle_school
            2. high_school
            3. college
            4. academy

            Examples:
            {{$examples}}
            ---

            Instructions for GPT-3.5:
            1. Read the provided learning objective and understand the language it is written in.
            2. Determine the primary cognitive process or skill required by the objective and select the appropriate Bloom's Taxonomy level based on the cognitive process (in the answer you have to put the corresponding number).
            4. Select the appropriate Level from the list for the learning objective based on the context (in the answer you have to put the corresponding number).
            5. Include, **in the same language of the provided learning objective, like the examples**, the macro-subject, and the topic associated with the learning objective.
            6. Format the answer in JSON format as shown in the examples.";
       
        public static string LearningObjectiveGenerator = @"You're a {{$level}} teacher that wants to give a lesson to your {{$level}} level Learners about the topic of: ' {{$topic}} '. You need to choose the learning objective of the lesson. To do so, you need to generate between two suitable learning objectives for each Bloom's Taxonomy level; each of them should be concise, specific, and aligned with the requested topic. {{$context}}
        Examples: {{$examples}}
        ---
        Instructions for the generation of the learning objectives:
        1. Understand the language of the provided topic. Your learning objectives must be in the same language.
        2. If some context is context is specified, consider it while generating the learning objectives.
        3. Generate the learning objectives for each Bloom's Taxonomy level: Remembering, Understanding, Applying, Analyzing, Evaluating, and Creating. Each learning objective should start with 'Learners will be able to ' (or its equivalent in the extracted language).
        4. Translate those learning objectives into the topic's language.
        5. Return the learning objectives in JSON format as shown below (Keep the keys in English and the learning objectives in topic's language). Provide **ONLY** the JSON. 
        Format: {{$format}}
    }";
    }
    
    public class SyllabusPrompts{
      public static string SyllabusGenerator = @"You are an expert in educational content creation. Your task is to generate a syllabus for a {{$macro_subject}} course on {{$title}}.
The course will cover the following topics: {{$topics}}.
The course aims to achieve a specific level of Bloom's taxonomy. **ALL** the 'Learning Outcomes' and **ALL** the 'Course Goals' **MUST** be aligned with **ONLY** one of the following Bloom's Taxonomy levels:
-{{$bloom_levels}}Avoid using verbs that belong to higher-order Bloom's taxonomy levels than requested.

The syllabus must be formatted in JSON using the following structure: {{$format}}.
Ensure that the content is clear, concise, and appropriate for a {{$level}} level of study.

The structure of the output should adhere to this JSON format: {{$format}}.

Here are some valid examples for reference:
{{$examples}}

**Provide ONLY the JSON output.**";
    }
    public class PlanningPrompts{
      public static string LessonPlannerprompt = @"You're a {{$level}} {{$marco_subject}} skilled assistant-teacher. The main teacher asked you to plan the next lesson, which is about: ' {{$title}} '. The teacher defined the learning objective for the lesson to be: '{{$learning_objective}}' which reflect the {{$bloom_level}} level in Bloom's taxonomy. 
You now need to define the lesson plan completely in English. From your previous lessons, you wrote down some notes about the class behaviour. Notes: '{{$context}}' (note that these notes are in {{$language}}, so you have to translate them into English to understand the context).
Since you're a very organized person, you want to structure the lesson plan in a logical sequence of nodes. A node is an element that consists of: Type (either 'Lesson' or 'Activity'), Topic (chosen from the list provided from the teacher), Description (either an activity category from the list or a suggestion of how to explain the lesson node to the Learners considering your Notes), Duration (which is the time in minutes that you expect to spend on that node).
To stick to the required Bloom level, consider the following verbs-level association:
### Specific Restrictions Based on the Bloom's Taxonomy Level:
- **Remembering**: Define, List, Recall, Identify, Recognize, Retrieve
- **Understanding**: Illustrate, Explain, Summarize, Describe, Classify, Categorize
- **Applying**: Respond, Provide, Use, Demonstrate, Solve, Apply, Implement
- **Analyzing**: Select, Distinguish, Analyze, Differentiate, Compare, Contrast, Integrate, Deconstruct, Structure
- **Evaluating**: Judge, Critique, Justify, Determine, Monitor, Detect, Reflect
- **Creating**: Design, Create, Formulate, Hypothesize, Assemble, Construct

To provide the teacher a clear and organized lesson plan, you decided to structure it in a JSON format as it follows. Format: {{$format}}
To help you in this job, the teacher provided you with a list of topics and a list of activities categories to use in the lesson plan and some examples of valid lesson plans.
Here are the topics: {{$topics}}
Here are the activities categories: {{$activities}}
Here are the examples: {{$examples}}
You now can plan the lesson seamlessly integrating the topics with some activities in the lesson plan. The last node MUST be an 'Activity' node, as it serves as final assessment.
Provide **ONLY** the JSON of the lesson plan, completely in English like the examples.";
    
      public static string AbstractNodePrompt = @"You're a {{$level}} {{$marco_subject}} skilled teacher. One of your Learners just completed an assessment about: ' {{$title}} '.
After correcting the assessment, you wrote feedback for the student: '{{$correction}}'. Now, strictly based on this feedback, you want to provide the student with a tailored lesson plan to help him understand the topics better. To create this tailored plan, you need to focus only on the topics that the student needs to re-study.
As a highly organized teacher, you want to structure the mini-lesson plan logically as a sequence of nodes. A node consists of Type (either 'Lesson' or 'Activity'), Topic (the topic the student needs to re-study), Description (an activity category from the list or a brief description of the topic), and Duration (time in minutes you expect to spend on that node).
To provide a clear and organized lesson plan, you decided to structure it in a JSON format. Format: {{$format}}
You have the list of activities categories to use in the lesson plan and some examples of valid tailored lesson plans.
Here are the activities categories: {{$activities}}
Here are the examples: {{$examples}}
You should ensure that the plan is concise, that it covers only the necessary topics and thet each topic is properly linked to at least one Activity node serving as a assessment.
Provide **ONLY** the JSON of the lesson plan, completely in English like the examples.";
    
      public static string CoursePlanPrompt = @"
You are a skilled {{$macro_subject}} teacher at the {{$level}} level, tasked with designing a course titled '{{$title}}'. 
This course must consist of exactly {{$number_of_lessons}} lessons, each lasting {{$lesson_duration}} minutes.
Here is the list of the micro topics you need to include in the course: {{$topics}}
Your goal is to develop a structured {{$level}} level course plan that effectively covers all the provided micro-topics in exactly {{$number_of_lessons}} lessons.

Here's what you need to do:
1. **Group and Sequence Topics**: Group and organize the provided micro-topics into {{$number_of_lessons}} lessons. Ensure the topics within each lesson are logically sequenced.
2. **Balance Lesson Content**: Arrange and plan the {{$number_of_lessons}} lessons, making sure each lesson is balanced in terms of content and time allocation ({{$lesson_duration}} minutes per lesson). You must also consider the time needed for exercises and class discussion, thus the lessons should not be too dense. If you think it's appropriate, you can dedicate some entire lessons to testing, feedbacks or other activities that are not striclty teaching new topics.
3. **Output Format**: Present the lesson plan in a JSON format, with keys in English and values in {{$language}}. The structure should align with the provided format: {{$format}}.

**Important**:
- Focus on logical progression and coherence in each lesson.
- Provide **ONLY** the JSON output.";
    }
    public class EnhancerPrompts{
      public static string RequestValidationPrompt = @"""JSON: {{$json}}
REQUEST:{{$request}}

### Instructions for the LLM:

1. **Task**: You are given two inputs:
   - A JSON object containing fixed keys with modifiable values.
   - A free-text request asking for specific improvements to the **values** of the provided JSON, where **keys must remain unchanged**.

2. **Validation**:
   - **Check the request**: Ensure that the request asks for improvements to the values of the JSON and that it is related to enhancing or modifying the values.
   - **Criteria for valid requests**:
     - The request must ask for improvements or suggestions to modify **only the values**.
     - The request should focus on aspects like clarity, appropriateness, relevance, or optimization of the values in a specific context (e.g., business, technical, etc.).
   
3. **Response Handling**:
   - If the request is **valid**, return a **refined version** of the request in {{$language}} considering best practices for prompt engineering.
   - If the request is **invalid** (e.g., it asks to change keys, or is unrelated to improving the values, it's not applicable to the json), return an error message in {{$language}}. The error message should always start with ""ERROR"" and clearly explain why the request is invalid.

4. **Output**:
    - Provide a JSON response with the key ""response"" containing the refined request or an error message.

### Example Outputs:

1. **Valid Example**:
     JSON: {
       ""name"": ""John Doe"",
       ""role"": ""Manager"",
       ""department"": ""Marketing""
     }
     REQUEST: ""Can you refine the values to be more suitable for an executive-level employee?""

   **Expected response**:
   {
     ""response"": ""Please review the values in the provided JSON and suggest changes to make them more suitable for an executive-level employee, keeping the keys the same.""
   }

2. **Invalid Example**:
     JSON: {
       ""product"": ""Laptop"",
       ""price"": ""$999"",
       ""stock"": ""50 units""
     REQUEST: ""Can you change the key names to something simpler?""
   }

   **Expected response**:
   {
     ""response"": ""ERROR: The request asks to modify the keys, which is not allowed. The request should focus on improving the values while keeping the keys unchanged.""
   }
""";
      public static string EnhancerPrompt = @"JSON: {{$json}}
REQUEST: {{$request}}

### Instructions for the LLM:

1. **Task**: You are provided with a JSON object and a REQUEST asking for improvements or enhancements to the **values** in the JSON.
   - The **keys in the JSON** must remain unchanged.
   - Focus on refining the **values** according to the context provided in the request and to the language of the values, which is {{$language}}.

2. **Output**:
   - Return only the **refined JSON** with the updated values in {{$language}}, based on the request. It's crucial for post processing that the key remain unchanged and that you return only a code section containing the final refined JSON.";
    }
    public class ExerciseCorrectorPrompt{
      public static string ExerciseCorrector = @"The output should fit the format: {{$format}}
Here are some examples: {{$examples}}
You are a teacher and you need to evaluate your Learners. You asked them: Question:'{{$question}}'. To that question they answered with: Answer:'{{$answer}}'.
Considering that you expected an answer like: Expected answer: '{{$expected_answer}}', evaluate the accuracy of your Learners' answer and, eventually, give them a feedback about what they did wrong, why it is wrong and how they should correct their answer.
Keep in mind that the accuracy value must range from 0 (if the answer is completely wrong) to 1 (if the answer is completely correct) with 0.2 intervals, where 0.0 and 0.2 are for a wrong answer, 0.4 and 0.6 are for a partially correct answer, 0.8 and 1.0 are for a correct answer.
Keep in mind that the language of the eventual correction must match the language of the question and answer.
Provide **ONLY** the JSON with accuracy and correction.
";
    }
    public class InternalPrompts {
        public static string TextSummarizationPrompt = @"You are a {{$level}} teacher that just gave a lesson and now you need to give a brief summary of it to your Learners as recap. This is your lesson material '{{$material}}'.
----
Instructions for GPT:
1. Read the provided material and understand the language it is written in.
2. Summarize the material, maintaining the same language (this is very important, if you provide a summary in English, even if the material is not in English, Learners won't be able to understand it), in {{$n_o_w}} words for {{$level}} level learners, incorporating all essential concepts and eventual formulas seamlessly.
3. Provide **ONLY** the synthesized content.";
        public static string TextTranslationPrompt = @"As a native {{$language}} speaker fluent in both {{$language}} and English languages, translate the following JSON's values in {{$language}} while keeping the keys in English: JSON:
{{$json}}
Provide ONLY translated JSON.";
        public static string MaterialGenerationPrompt = @"As a {{$level}} level professor, create a {{$level}} level lesson on: '{{$topic}}' for your {{$level}} level Learners. 
Craft a {{$n_o_w}}-words lesson with the learning objective: '{{$learning_objective}}' and utilizing appropriate {{$level}} vocabulary.
----
Instructions for GPT:
1. Read the provided topic and the learning objective to understand the language they are written in; that's the language you lesson must be in, as your Learners understand only that language.
2. Generate the lesson, maintaining the same language (this is very important, if you provide a lesson in English, even if the topic and learning objective are not in English, Learners won't be able to understand it), in {{$n_o_w}} words for {{$level}} level learners, incorporating all essential concepts and eventual formulas seamlessly.
3. Provide **ONLY** the generated content.";  
    }
    public class FormatStrings{
        public static string MM_FillInTheBlanks = @"
{
    ""Assignment"": ""assignment phrase"",
    ""Plus"": ""text about the topic, like the examples"",
    ""Solutions"": [
        ""solution 1"",
        ""solution 2"",
        ...
        ""solution n""
    ],
    ""Distractors"": [
        ""distractor 1"",
        ""distractor 2"",
        ...
        ""distractor n""
    ],
    ""EasilyDiscardableDistractors"": [
        ""easily discardable distractor 1"",
        ""easily discardable distractor 2"",
        ...
        ""easily discardable distractor n""
    ]
}";
        public static string MM_Question = @"
{
    ""Assignment"": ""assignment of the question exercise, it contains the assignment phrase/question"",
    ""Solutions"": ""solution of the question exercise, it contains the correct response to the assignment """"
}";
        public static string MM_Choice = @"
{
    ""Assignment"": ""assignment phrase/question"",
    ""Plus"": ""explanation of the correct answer/answers'"",
    ""Solutions"": [
        ""solution 1"",
        ""solution 2"",
        ...
        ""solution n""
    ],
    ""Distractors"": [
        ""distractor 1"",
        ""distractor 2"",
        ...
        ""distractor n""
    ],
    ""EasilyDiscardableDistractors"": [
        ""easily discardable distractor 1"",
        ""easily discardable distractor 2"",
        ...
        ""easily discardable distractor n""
    ]
}";
        public static string MM_Conceptual = @"
{
    ""Assignment"": ""assignment of the conceptual exercise, it contains the assignment phrase"",""
}";
        public static string MM_Practical = @"
{
    ""Assignment"": ""assignment of the practical exercise, it contains the assignment phrase"",""
}";
  
        public static string LO_Format = @"
""{
  ""Remembering"": [],
  ""Understanding"": [],
  ""Applying"": [],
  ""Analyzing"": [],
  ""Evaluating"": [],
  ""Creating"": []
}""";
        public static string SyllabusFormat = @"""{
  ""CourseTitle"": ""Short and descriptive title of the course"",
  ""CourseDescription"": ""Brief overview of the course, from where it starts to where it ends and what to expect"",
  ""LearningOutcomes"": [ //List of all the most important learning outcomes
    ""Learning outcome 1"",
    ""Learning outcome 2"",
    ...
    ""Learning outcome n""
  ],
  ""CourseGoals"": [ //List of all the goals of the course
    ""Course goal 1"",
    ""Course goal 2"",
    ...
    ""Course goal n""
  ],
  ""CourseTopics"": [
    {
      ""Topic"": ""Topic 1 Title"",
      ""Description"": ""Description of the topic 1""
    },
    {
      ""Topic"": ""Topic 2 Title"",
      ""Description"": ""Description of the topic 2""
    },
    ....
    {
      ""Topic"": ""Topic n Title"",
      ""Description"": ""Description of the topic n""
    }
  ],
  ""Prerequisites"": [ //List of all the non trivial prerequisites for the course
    ""Pre-requisite 1"",
    ...
    ""Pre-requisite n""
  ]
}""";
        public static string ExerciseCorrectionsFormat = @"
{
""Accuracy"": double value that represents the accuracy of the answer ,
""Correction"": ""explanation (in question's and answer's language) of what was wrong with the answer and why it was wrong. Write 'null' if accuracy < 0.8""
}";
    
        public static string AnalyserFormat = @"
{
  ""Language"": ""the language in which the material is written"",
  ""MacroSubject"": ""macro-subject of the material, such as history, math, literature, etc.."",
  ""Title"": ""generate a title that best summarizes the material and reflects its main focus and content"",
  ""PerceivedDifficulty"": ""assess the perceived level of difficulty of the material and provide a rating or description indicating its complexity.The perceived level should fit one of the following categories: primary_school, middle_school, high_school, college, or academy"",
  ""MainTopics"": [  extract all the N main topics covered in the material, provide a list of objects with the following stucture:
    {
      ""Topic"": ""generate an explicative short title in English for the first topic"",
      ""Description"": ""description of the first topic"",
      ""Type"": ""type of the first topic"",
      ""Bloom"": ""Bloom's level associated with the first topic (""Remembering"", ""Understanding"", ""Applying"", ""Analyzing"", ""Evaluating"", ""Creating"")"",
      ""Start"": ""index of the first char of the first topic"",
      ""End"": ""first five words of the first topic"",
      ""Keywords"": [""keyword1"", ""keyword2"", ...]  list of keywords related to the first topic
    },
    {
      ""Topic"": ""generate an explicative short title in English for the second topic"",
      ""Description"": ""description of the second topic"",
      ""Type"": ""type of the second topic"",
      ""Bloom"": ""Bloom's level associated with the second topic (""Remembering"", ""Understanding"", ""Applying"", ""Analyzing"", ""Evaluating"", ""Creating"")"",
      ""Start"": ""first five words of the second topic"",
      ""Keywords"": [""keyword1"", ""keyword2"", ...]  list of keywords related to the second topic
    },
    ....
    {
      ""Topic"": ""generate an explicative short title in English for the third topic"",
      ""Description"": ""description of the nth topic"",
      ""Type"": ""type of the nth topic"",
      ""Bloom"": ""Bloom's level associated with the nth topic (""Remembering"", ""Understanding"", ""Applying"", ""Analyzing"", ""Evaluating"", ""Creating"")"",
      ""Start"": ""first five words of the nth topic"",
      ""Keywords"": [""keyword1"", ""keyword2"", ...]  list of keywords related to the nth topic
    }
  ]
}";
    
        public static string LessonPlanFormat = @"
{
  ""lesson_plan"": [
    {
      ""type"": ""either Lesson or Activity"", 
      ""topic"": ""topic of the node 1"",
      ""description"": ""either suggestion on how to explain the lesson node or value of activity-category"",
      ""duration"": int
    },
    {
      ""type"": ""either Lesson or Activity"", 
      ""topic"": ""topic of the node 2"",
      ""description"": ""either suggestion on how to explain the lesson node or value of activity-category"",
      ""duration"": int
    },
    ...
    {
      ""type"": ""either Lesson or Activity"", 
      ""topic"": ""topic of the node n"",
      ""description"": ""either suggestion on how to explain the lesson node or value of activity-category"",
      ""duration"": int
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""topic of final activity"",
      ""description"": ""value of activity category"",
      ""duration"": int
    }
  ]
}";
    
        public static string CoursePlanFormat = @"
{
  ""plan"": [
    {
      ""title"": ""title of lesson 1"",
      ""topics"": [
        ""first subtopic"",
        ""second subtopic"",
        ...
        "" nth subtopic""
      ]
    },
    {
      ""title"": ""title of lesson 2"",
      ""topics"": [
        ""first subtopic"",
        ""second subtopic"",
        ...
        "" nth subtopic""
      ]
    },
    ...
    {
      ""title"": ""title of lesson n"",
      ""topics"": [
        ""first subtopic"",
        ""second subtopic"",
        ...
        "" nth subtopic""
      ]
    }
  ]
}";
    
    }
    public class ExamplesStrings{
        public static string FillInTheBlanks = @"Example of a high school theoretical exercise about Pearl Harbour:
{
  ""Assignment"": ""Pearl Harbor, one of the most significant events of World War II, is often remembered for its surprise attack on the United States. Test your understanding of this historical event by filling in the blanks with the correct words."",
  ""Plus"": ""On December 7, 1941, the Imperial Japanese Navy launched a surprise attack on the United States naval base at Pearl Harbour, Hawaii. The attack took place early in the morning and targeted the American Pacific Fleet. This event led to the United States' entry into World War II. The Japanese launched their attack with aircraft carriers and struck multiple ships, including the USS Arizona, which sank with a great loss of life. The attack was a devastating blow to the US Navy and caught the American military completely off guard. The following day, President Franklin D. Roosevelt famously declared that day as 'a date which will live in infamy.'"",
  ""Solutions"": [
    ""December 7"",
    ""1941"",
    ""Hawaii"",
    ""World War II"",
    ""USS Arizona"",
    ""Franklin D. Roosevelt"",
    ""infamy""
  ],
  ""Distractors"": [
    ""July 12"",
    ""1942"",
    ""Utah"",
    ""World War I"",
    ""USS Carrier"",
    ""Kennedy"",
    ""shame""
  ],
  ""EasilyDiscardableDistractors"": [
    ""February 29"",
    ""1965"",
    ""Germany"",
    ""cold war"",
    ""Titanic"",
    ""Lincoln"",
    ""peace""
  ]
}

Example of a college level code exercise about Java abstract classes and interfaces:
{
""Assignment"": ""Test your understanding of abstract classes and interfaces by filling in the blanks with the correct Java terms."",
""Plus"": ""abstract class Shape {\n    private String color;\n\n    public Shape(String color) {\n        this.color = color;\n    }\n\n    public abstract double area();\n\n    public String getColor() {\n        return color;\n    }\n}\n\ninterface Drawable {\n    void draw();\n}\n\nclass Circle extends Shape implements Drawable {\n    private double radius;\n\n    public Circle(String color, double radius) {\n        super(color);\n        this.radius = radius;\n    }\n\n    @Override\n    public double area() {\n        return Math.PI * radius * radius;\n    }\n\n    @Override\n    public void draw() {\n        System.out.println(''Drawing Circle with color: '' + getColor());\n    }\n}"",
""Solutions"": [
    ""abstract class"",
    ""extends"",
    ""implements"",
    ""@Override"",
    ""double"",
    ""@Override""
],
""Distractors"": [
    ""class"",
    ""implement"",
    ""uses"",
    ""@Redefine"",
    ""abstract double"",
    ""@Redefine""
],
""EasilyDiscardableDistractors"": [
    ""object"",
    ""ofClass"",
    ""interfaces"",
    ""@Nullable"",
    ""int"",
    ""@Nullable""
]
}
        
Example of a high school level problem exercise about parabolic motion:
{
""Assignment"": ""A projectile is launched from the ground with an initial velocity of v0 at an angle of theta degrees above the horizontal. During its flight, the projectile follows a path described by a parabola. Consider friction null."",
""Plus"": ""To analyze its motion, we can utilize the equations derived from the equations of motion. The horizontal and vertical components of the projectile's motion can be determined separately. The horizontal motion is uniform, with the velocity v0x remaining constant. The horizontal distance traveled (x) can be calculated using the equation x = v0x * t, where v0x = v0 * cos(theta) is the initial horizontal velocity. The vertical motion is influenced by gravity, causing the projectile's vertical velocity vy to change over time. The vertical position (y) at any time t can be determined using the equation y = v0y * t - 0.5 * g * t^2, where v0y = v0 * sin(theta) is the initial vertical velocity and g is the acceleration due to gravity. The maximum height (H) reached by the projectile can be found by setting the vertical velocity (vy) to zero, resulting in the equation H = v0y^2 / (2 * g). Finally, the time of flight (T) for the projectile can be calculated using the equation T = 2 * v0y / g, where v0y is the initial vertical velocity and g is the acceleration due to gravity."",
""Solutions"": [
    ""uniform"",
    ""remaining constant"",
    ""x = v0x * t"",
    ""v0x = v0 * cos(theta)"",
    ""gravity"",
    ""y = v0y * t - 0.5 * g * t^2"",
    ""v0y = v0 * sin(theta)"",
    ""H = v0y^2 / (2 * g)"",
    ""T = 2 * v0y / g""
],
""Distractors"": [
    ""varible"",
    ""decreasing over time"",
    ""x = 1/2 * v0x * t^2"",
    ""v0x = v0 * sin(theta)"",
    ""wheight force"",
    ""y = 1/2 * v0y * t - * g * t^2"",
    ""v0y = v0y * cos(theta)"",
    ""H = v0y / (2 * g)"",
    ""T = v0y / g""
],
""EasilyDiscardableDistractors"": [
    ""unpredictable"",
    ""change randomly over time"",
    ""x = random"",
    ""v0x = v0 * tan^2(theta)"",
    ""wind"",
    ""y = 1/2 * v0y^2 * t + * g * t^4"",
    ""v0y = v0y * tan^3(theta)"",
    ""H = v0x / g"",
    ""T = v0y / v0x""
]
}

Example of a high school level theoretical/problem resolution exercise about Bernoulli's principle and the formula for fluids:
{
""Assignment"": ""Test your understanding of Bernoulli's principle and the formula for fluids by filling in the blanks with the correct terms."",
""Plus"": ""Bernoulli's principle states that in a flowing fluid, an increase in the speed of the fluid occurs simultaneously with a decrease in pressure or a decrease in the fluid's potential energy. This principle finds application in various fluid dynamics scenarios, such as in the airflow around an aircraft wing or in the flow of water through a pipe. The mathematical expression of Bernoulli's principle is given by the equation: P + 0.5 * ρ * v^2 + ρ * g * h = constant, where P is the pressure, ρ is the density of the fluid, v is the velocity of the fluid, g is the acceleration due to gravity, and h is the height of the fluid above a reference point."",
""Solutions"": [
    ""Bernoulli's principle"",
    ""fluids"",
    ""P + 0.5 * ρ * v^2 + ρ * g * h = constant""
],
""Distractors"": [
    ""Pascal's theorem"",
    ""gases"",
    ""P + ρ * g * h = constant""
],
""EasilyDiscardableDistractors"": [
    ""Archimedes' principle"",
    ""solids"",
    ""P + v^2 + g * h = constant""
]
}

Example of a middle school level theoretical exercise about turtles:
{
""Assignment"": ""Test your understanding of turtles biology facts by filling in the blanks with the correct terms."", 
""Plus"": ""Turtles are reptiles characterized by their bony or cartilaginous shell. They are known for their slow movement and can be found in various habitats, including oceans, rivers, and forests. Turtles have a unique anatomy, with their shell serving as a protective covering for their body. They are ectothermic, meaning their body temperature is regulated by external sources of heat. Turtles are oviparous, laying eggs to reproduce, and some species can live for several decades."",
""Solutions"": [
    ""reptiles"",
    ""shell"",
    ""ectothermic"",
    ""oviparous""
],
""Distractors"": [
    ""mammals"",
    ""fur"",
    ""endothermic"",
    ""viviparous""
],
""EasilyDiscardableDistractors"": [
    ""birds"",
    ""feathers"",
    ""amphibious"",
    ""herbivorous""
]
}
";
        public static string SingleChoice= @"Example about a high school level problem solving exercise about parabolic motion:
{
""Assignment"": ""A ball is launched vertically upward with an initial velocity of 15 m/s. Neglecting air resistance, calculate the maximum height reached by the ball using the parabolic motion formula."",
""Plus"": ""To find the maximum height reached by the ball, we need to determine the vertical position \( y \) when the velocity \( v \) is 0. We can use the parabolic motion formula to calculate this. \n Given: Initial velocity, \( v_0 = 15 \) m/s \n Acceleration due to gravity, \( g = 9.8 \) m/s\(^2\) \n Using the parabolic motion formula \( y = v_0 t - \frac{1}{2}gt^2 \), where \( v = 0 \) at maximum height, we have:\n \[ 0 = 15t - \frac{1}{2}(9.8)t^2 \] \n Solving this quadratic equation will give us the time \( t \) it takes for the ball to reach its maximum height. Once we find \( t \), we can substitute it back into the formula to find the maximum height \( y \)."",
""Solutions"": [
    ""11.53 m""
],
""Distractors"": [
    ""10.35 m"",
    ""12.34 m""
],
""EasilyDiscardableDistractors"": [
    ""9.8 m"",
    ""13.5 m""
]
}

Example of a high school level code exercise about dictionaries in Python:""
{
""Assignment"": ""Given the Python code below, which of the following is the correct output? \n student_grades = {''John'': 85, ''Emily'': 92, ''Michael'': 78}\n print(student_grades[''John''])"",
""Plus"": ""The provided code defines a dictionary student_grades with Learners' names as keys and their corresponding grades as values. It then prints the grade of the student named ''John'' using square brackets notation to access the value associated with the key ''John''."",
""Solutions"": [
    ""85""
],
""Distractors"": [
    ""92"",
    ""''John''""
],
""EasilyDiscardableDistractors"": [
    ""78"",
    ""''Michael''""
]
}

Example of a middle school level theoretical exercise about Shakespeare's Hamlet:
{
""Assignment"": ""What is the famous opening line of William Shakespeare's play, Hamlet?"",
""Plus"": ""The famous opening line of Hamlet is ''To be, or not to be?'' It is one of the most iconic lines in English literature, encapsulating the central theme of existentialism and the internal conflict faced by the protagonist, Prince Hamlet."",
""Solutions"": [
    ""''To be, or not to be?''""
],
""Distractors"": [
    ""''To live, or to die?''"",
    ""''To exist, or not?''"",
    ""''To be, or to die?""''
],
""EasilyDiscardableDistractors"": [
    ""''To love, or to hate?''""
]
}

Example of a college level code exercise about error recognition in C++ linked lists:
{
""Assignment"": ""Identify the error in the following C++ code snippet related to linked lists:\n\n#include <iostream>\n\nstruct Node {\n    int data;\n    Node* next;\n};\n\nNode* createNode(int value) {\n    Node* newNode = new Node;\n    newNode->data = value;\n    newNode->next = nullptr;\n    return newNode;\n}\n\nvoid insertNode(Node*& head, int value) {\n    Node* newNode = createNode(value);\n    if (head == nullptr) {\n        head = newNode;\n        return;\n    }\n    Node* temp = head;\n    while (temp->next != nullptr) {\n        temp = temp->next;\n    }\n    temp->next = newNode;\n}\n\nvoid displayList(Node* head) {\n    Node* temp = head;\n    while (temp != nullptr) {\n        std::cout << temp->data << ' ';\n        temp = temp->next;\n    }\n    std::cout << std::endl;\n}\n\nint main() {\n    Node* head = nullptr;\n    insertNode(head, 10);\n    insertNode(head, 20);\n    insertNode(head, 30);\n    displayList(head);\n    return 0;\n}"",
""Plus"": ""The error in the code is in the insertNode function. After creating a new node, the function is not properly updating the next pointer of the last node in the linked list to point to the newly created node. This results in the insertion of new nodes always at the end of the linked list, instead of correctly linking them in order."",
""Solutions"": [
    ""Modify insertNode function to correctly update next pointer of the last node.""
],
""Distractors"": [
    ""Add head->next = newNode; after head = newNode;"",
    ""Change temp->next = newNode; to temp = newNode;"",
    ""Insert return; after temp->next = newNode;""
],
""EasilyDiscardableDistractors"": [
    ""Remove temp->next = newNode; from insertNode function.""
]
}";
        public static string MultipleChoice = @"Example of a high school level problem solving exercise about quadratic equations resolution:
{
  ""Assignment"": ""Solve the quadratic equation x^2 - 5x + 6 = 0 to find the zeros of the corresponding parabola."",
  ""Plus"": ""To find the zeros of the quadratic equation x^2 - 5x + 6 = 0, we can use the quadratic formula, x = (-b ± √(b^2 - 4ac)) / (2a), where a = 1, b = -5, and c = 6."",
  ""Solutions"": [
    ""x = 2"",
    ""x = 3""
  ],
  ""Distractors"": [
    ""x = 4"",
    ""x = 1""
  ],
  ""EasilyDiscardableDistractors"": [
    ""x = -2"",
    ""x = 5""
  ]
}

Example of a college level code exercise about polymorphism in C#:
{
  ""Assignment"": ""Identify the errors in the following C# code snippet related to polymorphism:\n\nclass Animal {\n    public virtual void MakeSound() {\n        Console.WriteLine(''Some generic sound'');\n    }\n}\n\nclass Dog : Animal {\n    public override void MakeSound() {\n        Console.WriteLine(''Woof'');\n    }\n}\n\nclass Cat : Animal {\n    public override void MakeSound() {\n        Console.WriteLine(''Meow'');\n    }\n}\n\nclass Program {\n    static void Main(string[] args) {\n        animal1 = new Dog();\n        Animal animal2 = new Cat();\n        \n        animal1.Bark(); \n   animal1.MakeSound();\n        animal2.MakeSound()\n    }\n}"",
  ""Plus"": ""Error: animal1 = newDog(); //type of animal1 is not defined\nError:  animal1.Bark(); // animal1 does not have a method Bark()\nError: animal2.MakeSound() // missing ; at the end of the line"",
  ""Solutions"": [
    ""animal1 = newDog();"",
    ""animal1.Bark();"",
    ""animal2.MakeSound()""
  ],
  ""Distractors"": [
    ""animal1.MakeSound();"",
    ""public virtual void MakeSound() {"",
    ""Animal animal2 = new Cat();""
  ],
  ""EasilyDiscardableDistractors"": [
    ""class Program {"",
    ""static void Main(string[] args) {"",
    "" public override void MakeSound() {"" 
  ]
}

Example of a college level code exercise about dictionaries in Python:
{
  ""Assignment"": ""Consider the following Python code snippet. What do the two outputs at the end represent?\n\nmy_dict = {''a'': 1, ''b'': 2, ''c'': 3}\n\nprint(my_dict[''a''])\n\nprint(my_dict.get(''d''))"",
  ""Plus"": ""The first output represents the value associated with the key ''a'' in the dictionary my_dict, which is 1. The second output represents the value associated with the key ''d'' in the dictionary my_dict, which does not exist, hence None is returned."",
  ""Solutions"": [
    ""1"",
    ""None""
  ],
  ""Distractors"": [
    ""''a'':1"",
    ""KeyError: 'd'""
  ],
  ""EasilyDiscardableDistractors"": [
    ""3"",
    ""''d''""
  ]
}

Example of a middle school level theoretical exercise about EU geograpy:
{
  ""Assignment"": ""Select the countries that are part of the European Union."",
  ""Plus"": ""Germany, France, and Italy are among the countries that are part of the European Union."",
  ""Solutions"": [
    ""France"",
    ""Germany""
  ],
  ""Distractors"": [
    ""Japan"",
    ""United Kingdom""
  ],
  ""EasilyDiscardableDistractors"": [
    ""Canada"",
    ""Australia""
  ]
}

Example of a high school level exercise about factors contributing to climate change:
{
  ""Assignment"": ""Identify factors contributing to climate change."",
  ""Plus"": ""Deforestation, burning fossil fuels, and industrial emissions are factors contributing to climate change."",
  ""Solutions"": [
    ""Deforestation"",
    ""Burning fossil fuels"",
    ""Industrial emissions""
  ],
  ""Distractors"": [
    ""Volcanic eruptions"",
    ""Ocean acidification""
  ],
  ""EasilyDiscardableDistractors"": [
    ""Photosynthesis"",
    ""Global cooling""
  ]
}";
        public static string ShortAnswerQuestion = @"Example of a high school level theoretical exercise about fluid dynamics: 
{
  ""Assignment"": ""What is the formula for calculating pressure in a fluid?"",
  ""Solution"": ""P = ρgh""
}

Example of a high school level problem solving exercise about fluid dynamics: 
{
""Assignment"": ""A cylindrical tank with a diameter of 2 meters is filled with water to a height of 3 meters. Calculate the pressure at the bottom of the tank due to the weight of the water."",
""Solution"": ""29400 Pa""
}

Example about a high school level problem solving exercise about parabolic motion:
{
""Assignment: A ball is launched vertically upward with an initial velocity of 15 m/s. Neglecting air resistance, calculate the maximum height reached by the ball using the parabolic motion formula."",
""Solution"": ""11.53 m""
}

Example of a high school level code exercise about dictionaries in Python:
{
""Assignment"": ""Given the Python code below, which of the following is the correct output? \nstudent_grades = {''John'': 85, ''Emily'': 92, ''Michael'': 78}"",
""Solution"": ""85""
}

Example of a high school level theoretical exercise about Nietzsche:
{
""Assignment"": ""What was Nietzsche's first book?"",
""Solution"": ""''The Birth of Tragedy''""
}";
        public static string OpenQuestion = @" Example of a college level theoretical exercise about the fall of the roman empire:
{
  ""Assignment"": ""Explain the factors that contributed to the fall of the Roman Empire."",
  ""Solution"": ""The fall of the Roman Empire was a complex process that took place over several centuries. There were many factors that contributed to its decline and eventual collapse. One major factor was economic instability. The empire had become too large to be effectively governed, and as a result, corruption and inefficiency became rampant. This led to high taxes, inflation, and a decline in trade, which further weakened the economy. Another factor was military overspending. The empire's military campaigns were expensive, and as the empire expanded, it became increasingly difficult to maintain control over its vast territories. This led to a drain on resources and a decline in military effectiveness. Internal political instability also played a role in the fall of Rome. As the empire grew larger, it became more difficult to govern effectively. Political infighting and corruption weakened the government, making it less able to respond to external threats. External threats also contributed to Rome's decline. Barbarian invasions from Germanic tribes put pressure on the empire's borders, while attacks from Persians and other enemies weakened Rome's military power. Finally, cultural decay also played a role in Rome's fall. As Christianity spread throughout the empire, traditional Roman values began to erode. This led to a decline in civic virtue and patriotism, which further weakened the social fabric of the empire. In summary, the fall of Rome was caused by a combination of economic instability, military overspending, political instability, external threats, and cultural decay. These factors worked together over several centuries to weaken the empire until it eventually collapsed under its own weight.""
}

Example of a college level code exercise about linked lists creation in C++:
{
  ""Assignment"": ""Write a C++ program to create a linked list."",
  ""Solution"": ""#include <iostream>\n\nstruct Node {\n    int data;\n    Node* next;\n};\n\nclass LinkedList {\nprivate:\n    Node* head;\npublic:\n    LinkedList() {\n        head = nullptr;\n    }\n\n    // Function to insert a new node at the beginning of the list\n    void insert(int value) {\n        Node* newNode = new Node;\n        newNode->data = value;\n        newNode->next = head;\n        head = newNode;\n    }\n\n    // Function to display the linked list\n    void display() {\n        Node* temp = head;\n        while (temp != nullptr) {\n            std::cout << temp->data << ' ';\n            temp = temp->next;\n        }\n        std::cout << std::endl;\n    }\n};\n\nint main() {\n    LinkedList list;\n    list.insert(5);\n    list.insert(10);\n    list.insert(15);\n\n    std::cout << 'Linked List: ';\n    list.display();\n\n    return 0;\n}""
}

Example of a high school level problem solving exercise about fluid dynamics: 
{
  ""Assignment"": ""A cylindrical tank with a diameter of 2 meters is filled with water to a height of 3 meters. Calculate the pressure at the bottom of the tank due to the weight of the water."",
  ""Solution"": ""To calculate the pressure at the bottom of the tank, we'll use the formula for pressure due to the weight of a fluid: P = ρgh. Where: P is the pressure at the bottom of the tank, ρ is the density of the fluid (in this case, water), g is the acceleration due to gravity, and h is the height of the fluid column. First, let's find the density of water. At room temperature, the density of water is approximately 1000 kg/m^3. Given: Diameter of the tank, D = 2 m, Height of the water column, h = 3 m, Density of water, ρ = 1000 kg/m^3, and Acceleration due to gravity, g = 9.8 m/s^2. We need to find the pressure at the bottom of the tank, so we'll use the full height of the water column, h = 3 m. P = (1000 kg/m^3) × (9.8 m/s^2) × (3 m) = 29400 Pa. So, the pressure at the bottom of the tank due to the weight of the water is 29400 Pa.""
}


Example of a high school level theoretical exercise about fluid dynamics:
{
  ""Assignment"": ""Explain the concept of buoyancy and how it relates to the behavior of objects submerged in fluids."",
  ""Solution"": ""Buoyancy is the upward force exerted by a fluid on an object immersed in it. It is governed by Archimedes' principle, which states that the buoyant force acting on an object is equal to the weight of the fluid displaced by the object. When an object is placed in a fluid, it displaces some of the fluid, causing an upward force equal to the weight of the displaced fluid to act on the object. This buoyant force counteracts the weight of the object, causing it to feel lighter or even float if the buoyant force exceeds the weight of the object. Therefore, objects with a density greater than the fluid will sink, while those with a density less than the fluid will float.""
}
";
        public static string TrueOrFalse = @"Example of a college level exercise about Nietzsche:
{
  ""Assignment"": ""According to Nietzsche, the concept of the 'Übermensch' advocates for the superiority of a select group of individuals over others."",
  ""Solution"": ""False, Nietzsche's concept of the 'Übermensch' or 'Overman' does not promote the superiority of a select group of individuals over others. Instead, it emphasizes the idea of self-overcoming and the transcendence of conventional moral values and societal norms. The Übermensch represents the individual who can create their own values and live authentically, beyond the constraints of herd mentality or traditional morality. It is not about one group being superior to others, but rather about individual self-realization and autonomy.""
}

Example of a college level code exercise about queues and buffers in C++:
{
  ""Assignment"": ""In the following C++ code snippet, a queue is used to implement a buffer. Determine if the code will behave like a typical buffer. \n#include <iostream>\n#include <queue>\n\nusing namespace std;\n\nint main() {\n    queue<int> buffer;\n\n    // Adding data to the buffer\n    for (int i = 1; i <= 5; ++i) {\n        buffer.push(i);\n        cout << ''Added '' << i << '' to buffer.'' << endl;\n    }\n\n    // Removing data from the buffer\n    while (!buffer.empty()) {\n        int data = buffer.front();\n        buffer.pop();\n        cout << ''Removed '' << data << '' from buffer.'' << endl;\n    }\n\n    return 0;\n}"",
  ""Solution"": ""True, the provided code snippet demonstrates the use of a queue (std::queue) to implement a buffer in C++. The code adds data to the buffer using the push function and removes data from the buffer using the pop function, which follows the First In First Out (FIFO) order, typical of buffer behavior. Therefore, the code will behave like a typical buffer.""
}

Example of a high school level problem solving exercise about fluid dynamics:
{
  ""Assignment"": ""A ball is dropped into a container filled with water. As the ball sinks deeper into the water, the pressure it experiences increases."",
  ""Solution"": ""True, as the ball sinks deeper into the water, it experiences an increase in pressure due to the increasing weight of the water above it. This increase in pressure can be calculated using the hydrostatic pressure formula: P = ρgh, where P is the pressure, ρ is the density of the fluid (water in this case), g is the acceleration due to gravity, and h is the depth of the fluid. As the ball sinks deeper, the value of h increases, resulting in a higher pressure experienced by the ball. Therefore, the statement is true.""
}
";
        public static string Essay = @"Example of an academic level exercise about AI ethics in healthcare:
{
""Assignment"": ""Write an essay discussing the ethical considerations surrounding the use of artificial intelligence (AI) in healthcare.""
}

Example of an high school level exercise about AI impact:
{
""Assignment"": ""Write an essay discussing the impact of social media on middle school Learners.""
}

Example of an college level exercise about implications of climate change on global food security:
{
""Assignment"": ""Write an essay exploring the implications of climate change on global food security."" 
}

Example of an academic level exercise about AI impact:
{
""Assignment"": ""Write an essay analyzing the impact of artificial intelligence on employment trends in the 21st century.""
}

Example of a middle school level exercise about enviromental conservation:
{
""Assignment"": ""Write an essay discussing the importance of environmental conservation in your community.""
}

Example of a primary school exercise about favourite animals:
{
""Assignment"": ""Write a short essay describing your favorite animal and why you admire it.""
}
";
        public static string KnowledgeExposition = @"Example of a high school level exercise about civil war:
{
""Assignment"": ""Prepare a knowledge exposition on the causes, events, and consequences of the American Civil War. Consider the following requirements: \n 1) Your exposition should be well-organized and comprehensive, covering key aspects of the Civil War period.\n 2) Utilize a variety of sources, including primary documents, scholarly articles, and historical narratives, to support your exposition.\n 3) Present your findings in a clear and engaging manner, using visual aids such as maps, timelines, and images to enhance understanding.\n 4) Address the following aspects of the Civil War in your exposition: \n    a. Background and Context: Provide an overview of the political, economic, and social factors leading to the outbreak of the Civil War, including sectionalism, slavery, and states' rights.\n  b. Key Events and Battles: Discuss significant events and battles of the Civil War, highlighting their strategic importance and impact on the course of the conflict.\n     c. Leadership and Figures: Examine the leadership of key figures such as Abraham Lincoln, Jefferson Davis, Ulysses S. Grant, and Robert E. Lee, and their roles in shaping the outcome of the war.\n    d. Home Front and Society: Explore the experiences of civilians, soldiers, and marginalized groups during the Civil War, including the role of women, African Americans, and immigrants.\n  e. Legacy and Consequences: Analyze the long-term effects of the Civil War on American society, politics, and culture, including Reconstruction, the abolition of slavery, and the process of national reconciliation.\n    f. Historical Interpretations: Compare and contrast different historical interpretations of the Civil War, examining how perspectives on the conflict have evolved over time.\n 5) Conclude your exposition with a reflection on the enduring significance of the Civil War and its relevance to contemporary issues and challenges.""
}

Example of a college level exercise about statics:
{
""Assignment"": ""Demonstrate your knowledge on the topic of Statics by completing the following steps:\n 1) Select a set of statics problems covering topics such as equilibrium of particles and rigid bodies, moments, forces, and trusses. \n 2) Solve each problem analytically using appropriate mathematical techniques and principles of static equilibrium.\n 3) Present your solutions in a clear and organized format, including diagrams, free-body diagrams, and step-by-step calculations.\n 4) Discuss the physical significance of your results, including the implications for structural stability, load-bearing capacity, and engineering design.\n 5) Reflect on the broader applications of statics principles in engineering practice and real-world scenarios, highlighting their importance in ensuring the safety and reliability of structures and mechanical systems.""
}
";
        public static string NonWrittenMaterialAnalysis = @"Example of a middle school level exercise about arwork analysis:
{
""Assignment"": ""Analyze the given Artwork by following these steps:\n 1)Observe and describe the visual elements of the artwork, including color, composition, form, and texture. \n 2)Consider the mood or message conveyed by the artwork and how it makes you feel.\n 3)Research the artist and the historical context in which the artwork was created.\n Write a short comment discussing your interpretation of the artwork, its artistic techniques, and its significance in art history.""
}

Example of a high school level exercise about historical photograph interpretation:
{
""Assignment"": ""Interpret the given historical photograph by following these steps: \n 1)Examine the composition, subject matter, and context of the photograph.\n 2)Analyze the historical significance of the photograph, considering its impact on society and culture at the time of its creation. \n 3) Research the background information related to the photograph, including the photographer, location, and historical context.\n 4) Write a detailed interpretation of the photograph, discussing its symbolism, cultural relevance, and broader historical implications.""
}

Example of a college level exercise about sales dashboard analysis:
{
""Assignment"": ""Analyze the provided sales dashboard. Based on your analysis, formulate strategic recommendations for optimizing sales performance and achieving business objectives. Present your analysis and recommendations in a professional report format, addressing key stakeholders within the company and providing supporting evidence and rationale for your suggestions.""
}

Example of a college level exercise about financial statement analysis:
{
""Assignment"": ""Conduct a comprehensive analysis of the provided financial statements for a company. Follow these steps:\n 1) Review the balance sheet, income statement, and cash flow statement, examining key financial metrics such as revenue, expenses, assets, liabilities, and cash flow. \n 2) Evaluate the financial performance and position of the company, comparing current data to historical trends and industry benchmarks. \n 3) Identify strengths, weaknesses, opportunities, and threats (SWOT analysis) based on the financial data and market conditions.\n 4) Assess the company's profitability, liquidity, solvency, and efficiency ratios to gauge its financial health and operational effectiveness.\n 5) Formulate strategic recommendations for improving financial performance, managing risks, and achieving long-term growth objectives.\n 6) Present your analysis and recommendations in a professional report format, targeting key stakeholders such as investors, executives, and board members. Provide clear, concise explanations and supporting evidence to justify your conclusions.""
}
";
        public static string NonWrittenMaterialProduction = @"Example of a middle school level exercise about landscape painting:
{
""Assignment"": ""Using tempera paints, create a landscape painting depicting a scene of nature such as a forest, mountain range, or beach.""
}

Example of a high school level exercise about WWII concept map:
{
""Assignment"": ""Create a concept map illustrating the key events, causes, and consequences of World War II. Organize the concept map into main branches representing major themes such as political, military, social, and economic aspects of the war. Include sub-branches to further elaborate on specific events, battles, leaders, and countries involved in the conflict. Use arrows, connectors, and labels to demonstrate relationships and connections between different elements of the concept map.""
}

Example of a college level exercise about business plan creation:
{
""Assignment"": ""Create a comprehensive business plan PDF for a hypothetical business venture of your choice. Include sections such as executive summary, business description, market analysis, marketing and sales strategies, operations plan, and financial projections. \n Research industry trends, target market demographics, competitors, and regulatory requirements to inform your business plan. Use professional design software or templates to layout and format your business plan PDF for readability and visual appeal. Incorporate charts, graphs, and visual elements to illustrate key data and metrics supporting your business plan.""
}

Example of a college level exercise about data visualization:
{
""Assignment"": ""Using Tableau or a similar data visualization tool, create an interactive dashboard displaying key performance indicators (KPIs) for a fictional company. Import sample data sets or generate your own data to populate the dashboard with metrics such as revenue, expenses, profit margins, and customer satisfaction scores. Design the dashboard layout, color scheme, and visual elements to enhance data clarity and user engagement. Include interactive features such as filters, drill-down options, and tooltips to allow users to explore the data and gain insights from the visualizations.""
}

Example of a high school level exercise about presentation creation about the fall of the Roman Empire:
{
""Assignment"": ""Prepare a PowerPoint presentation on the fall of the Roman Empire. Create slides with informative content, engaging visuals, and clear organization to effectively communicate your message to the audience. Practice delivering the presentation with confidence and clarity, using speaking notes or cue cards to guide your presentation. Incorporate multimedia elements, such as images, videos, and animations, to enhance the visual appeal and interactivity of your slides.""
}
";
    
        public static string Debate = @"Example of a college level debate exercise about ethics in AI:
{
  ""Assignment"": ""Debate on the Ethical Implications of AI in Autonomous Vehicles\n\nInstructions:\nDivide the class into two groups: Team A and Team B.\nEach team will be assigned a stance on the ethical implications of AI in autonomous vehicles.\nTeam A will argue in favor of the statement: 'The benefits of AI in autonomous vehicles outweigh the ethical concerns.'\nTeam B will argue against the statement: 'The ethical concerns of AI in autonomous vehicles outweigh the benefits.'\nEach team will appoint a spokesperson to present their arguments.\nThe debate will consist of three rounds:\nRound 1: Opening Arguments\nRound 2: Rebuttals and Counterarguments\nRound 3: Closing Statements\nEach spokesperson will have three minutes to present their arguments in each round.\nDuring the rebuttal and counterarguments round, each team will have two minutes to respond to the opposing team's arguments.\nAfter all rounds are completed, the class will have an open discussion to further explore the nuances of the topic.\n\nDebate Points:\nTeam A (In favor of the benefits of AI in autonomous vehicles):\n- Safety: Argue that AI technology can significantly reduce the number of accidents caused by human error, thereby saving lives.\n- Efficiency: Highlight the potential for AI to optimize traffic flow, reduce congestion, and improve transportation systems overall.\n- Accessibility: Emphasize how autonomous vehicles can enhance mobility for individuals with disabilities and elderly populations.\n- Innovation: Discuss the role of AI in driving technological advancement and fostering economic growth in the automotive industry.\n\nTeam B (Against the ethical concerns of AI in autonomous vehicles):\n- Moral Dilemmas: Raise concerns about the ethical challenges surrounding AI decision-making in life-and-death situations, such as the 'trolley problem.'\n- Liability and Accountability: Address the complex legal and ethical issues related to assigning responsibility in the event of accidents or failures of autonomous vehicles.\n- Privacy: Explore the implications of AI surveillance in autonomous vehicles, including the collection and potential misuse of personal data.\n- Job Displacement: Discuss the socioeconomic impacts of AI-driven automation on employment in the transportation sector and the broader economy. \n- Objective: The objective of this debate exercise is to encourage Learners to critically analyze the ethical implications of AI in autonomous vehicles from multiple perspectives. By engaging in thoughtful discourse and considering both the benefits and concerns associated with this technology, Learners will develop a deeper understanding of the complex ethical dilemmas inherent in AI development and deployment.""
}

Example of a high school level exercise about impact of social media on teenagers:
{
  ""Assignment"": ""Debate on the Impact of Social Media Use on Teenagers\n\nInstructions:\nDivide the class into two groups: Team A and Team B.\nEach team will be assigned a stance on the impact of social media use on teenagers.\nTeam A will argue in favor of the statement: 'Social media has a positive impact on teenagers' lives.'\nTeam B will argue against the statement: 'Social media has a negative impact on teenagers' lives.'\nEach team will appoint a spokesperson to present their arguments.\nThe debate will consist of three rounds:\nRound 1: Opening Arguments\nRound 2: Rebuttals and Counterarguments\nRound 3: Closing Statements\nEach spokesperson will have three minutes to present their arguments in each round.\nDuring the rebuttal and counterarguments round, each team will have two minutes to respond to the opposing team's arguments.\nAfter all rounds are completed, the class will have an open discussion to further explore the complexities of social media's impact on teenagers.\n\nDebate Points:\nTeam A (In favor of the positive impact of social media on teenagers):\n- Connectivity: Argue that social media platforms facilitate communication and connection with peers, family members, and communities, fostering a sense of belonging and support.\n- Information Access: Highlight how social media provides teenagers with access to diverse perspectives, educational resources, and opportunities for self-expression and learning.\n- Networking: Discuss the role of social media in helping teenagers build professional networks, discover career opportunities, and develop essential digital skills for the modern workforce.\n- Empowerment: Emphasize the ability of social media to amplify teenagers' voices, promote activism, and catalyze social change by mobilizing youth-led movements.\n\nTeam B (Against the negative impact of social media on teenagers):\n- Mental Health: Raise concerns about the detrimental effects of excessive social media use on teenagers' mental well-being, including increased rates of anxiety, depression, and social comparison.\n- Cyberbullying: Address the prevalence of cyberbullying on social media platforms and its harmful consequences for victims, such as psychological distress, academic difficulties, and even suicide.\n- Addiction: Discuss the addictive nature of social media platforms, leading to decreased productivity, disrupted sleep patterns, and diminished real-life social interactions among teenagers.\n- Privacy and Security: Explore the risks of privacy breaches, online harassment, and exploitation of personal data on social media, posing threats to teenagers' safety and digital privacy rights.\n\nObjective:\nThe objective of this debate exercise is to encourage Learners to critically evaluate the impact of social media use on teenagers' lives from different perspectives. By engaging in informed discourse and considering both the positive and negative aspects of social media, Learners will develop a nuanced understanding of the complex dynamics shaping adolescents' digital experiences and well-being in the digital age.""
}
";
        public static string Brainstorming = @"Example of a college level exercise about contemporary ethical dilemmas:
{
""Assignment"": ""Brainstorming Session on Contemporary Ethical Dilemmas.\n What are some contemporary ethical dilemmas related to emerging technologies, such as artificial intelligence, genetic engineering, or biotechnology?\n How do cultural differences and moral relativism impact our understanding of ethics and moral decision-making in a globalized world? \n What are the ethical implications of environmental degradation and climate change, and how should societies address these challenges?\n How should we balance individual rights and societal interests in areas such as privacy, surveillance, and national security?\n What are the ethical considerations surrounding issues of social justice, inequality, and discrimination, and how can philosophy inform our responses to these challenges?\n How should we approach ethical dilemmas in healthcare, including topics such as end-of-life care, access to healthcare, and medical experimentation?\n What are the moral responsibilities of businesses and corporations in today's society, particularly concerning issues such as corporate social responsibility, environmental sustainability, and fair labor practices?\n How do ethical theories such as utilitarianism, deontology, virtue ethics, and existentialism inform our understanding of contemporary ethical dilemmas? \n What role should empathy, compassion, and moral imagination play in ethical decision-making and social change? \n How can philosophical reflection and ethical reasoning contribute to personal growth, moral development, and responsible citizenship in a complex and rapidly changing world?""
}

Example of an academic level exercise about AI robotics:
{
""Assignment"": ""Research Proposal on Ethical Considerations in AI Robotics.\n What are the key ethical considerations in the development and deployment of AI robotics?\n How do ethical frameworks such as utilitarianism, deontology, and virtue ethics apply to AI robotics?\n What are the potential risks and benefits of AI robotics in various domains, such as healthcare, transportation, and military applications?\n How should society address issues of accountability, transparency, and bias in AI robotics systems?\n What ethical guidelines and regulations exist for governing the use of AI robotics, and how effective are they?\n How can AI robotics contribute to addressing societal challenges, such as aging populations, environmental sustainability, and disaster response?\n What are the moral implications of AI robotics replacing human labor in the workforce, and how should societies mitigate potential socioeconomic impacts?\n What role should interdisciplinary collaboration play in addressing ethical concerns in AI robotics, involving fields such as philosophy, law, sociology, and computer science?\n How can AI robotics research and development incorporate principles of responsible innovation and ethical design?\n What are the ethical considerations surrounding autonomous decision-making by AI robotics systems, particularly in high-stakes situations where human lives may be at risk?""
}

Example of a middle school level exercise about endangered species conservation:
{
""Assignment"": ""Research Project on Endangered Species Conservation.\n What are endangered species, and why is it important to protect them?\n What are the main factors contributing to the decline of endangered species?\n Can you identify three endangered species and describe their habitats, behaviors, and unique characteristics?\n How do human activities, such as habitat destruction, pollution, and climate change, impact endangered species?\n What are some conservation efforts and initiatives aimed at protecting endangered species and their habitats?\n How can individuals contribute to endangered species conservation in their communities?\n What role do zoos, wildlife sanctuaries, and national parks play in endangered species conservation?\n How does biodiversity loss affect ecosystems and the balance of nature?\n Can you discuss the ethical considerations surrounding endangered species conservation, including conflicts between human development and wildlife conservation?\n What are the potential consequences of not taking action to protect endangered species for future generations?""
}
";
        public static string GroupDiscussion = @"Example of a college level group discussion about the impact of automation on employment:
{
  ""Assignment"": ""Engage in a group discussion on the impact of automation on employment.\n\nDiscussion Points:\n- How is automation transforming the nature of work across different industries?\n- What are the potential benefits of automation in terms of efficiency, productivity, and innovation?\n- What are the challenges and concerns associated with automation, such as job displacement, income inequality, and skills gaps?\n- How can societies and governments prepare for the impact of automation on the labor market?\n- What role should education, training, and lifelong learning play in helping individuals adapt to the changing demands of the workforce?\n- How can businesses and policymakers ensure that the benefits of automation are shared equitably, and that no one is left behind?\n- What are the ethical considerations surrounding the use of automation, particularly regarding job loss, economic stability, and social welfare?\n- How can automation be leveraged to create new job opportunities, enhance job quality, and promote economic prosperity?\n- What are the long-term implications of automation on the future of work, employment relations, and the overall structure of society?""
}
Example of a college level group discussion about the ethics of artificial intelligence:
{
  ""Assignment"": ""Engage in a group discussion on the ethical implications of artificial intelligence (AI).\n\nDiscussion Points:\n- What are the ethical considerations surrounding the development and use of AI technology?\n- How do AI algorithms impact issues such as privacy, bias, and autonomy?\n- What are the potential risks and benefits of AI in various domains, such as healthcare, finance, and criminal justice?\n- How should society address concerns about job displacement, inequality, and the ethical treatment of AI-generated data?\n- What ethical principles and frameworks should guide the development and deployment of AI systems?\n- How can we ensure transparency, accountability, and fairness in AI decision-making?\n- What role should interdisciplinary collaboration play in addressing ethical challenges in AI development?\n- How can individuals, organizations, and governments promote ethical AI practices and mitigate potential harms?\n- What are the long-term ethical implications of advancing AI technology, and how can we anticipate and address them proactively?""
}";
        public static string CaseStudyAnalysis = @"Example of an academic level case study analysis on the ethical implications of data privacy:
{
  ""Assignment"": ""Analyzing the ethical implications of data privacy violations by a social media company.\n\nCase Study Scenario:\nA major social media company is accused of violating user privacy by sharing personal data with third-party companies without user consent. This data includes personal information, browsing history, and user preferences, which were used for targeted advertising purposes.\n\nCase Study Analysis Questions:\n1. What are the ethical concerns raised by the social media company's actions?\n2. What are the potential consequences of these privacy violations for users and society as a whole?\n3. What ethical principles and values are at stake in this case, and how do they conflict with the business interests of the company?\n4. How should the social media company respond to allegations of data privacy violations?\n5. What role should government regulation and oversight play in protecting user privacy and holding companies accountable for data breaches?\n6. What steps can individuals take to protect their privacy online and hold companies accountable for unethical behavior?\n7. How do ethical theories such as utilitarianism, deontology, and virtue ethics inform our understanding of data privacy issues and guide ethical decision-making in this case?\n8. What are the broader implications of this case for the ethical use of data in the digital age, and how can society address these challenges moving forward?""
}
Example of a case study analysis on medical ethics:
{
  ""Assignment"": ""Analyzing the ethical considerations involved in organ transplant allocation.\n\nCase Study Scenario:\nA hospital is facing a shortage of donor organs for transplant surgeries. The hospital's transplant committee must decide how to allocate the available organs fairly and ethically.\n\nCase Study Analysis Questions:\n1. What are the ethical principles and values at stake in organ transplant allocation?\n2. What factors should the transplant committee consider when deciding how to allocate organs?\n3. How should the committee balance competing ethical concerns, such as medical need, recipient suitability, and distributive justice?\n4. What role should clinical criteria, such as medical urgency and likelihood of success, play in organ allocation decisions?\n5. How should the committee address issues of equity, fairness, and transparency in the organ allocation process?\n6. What are the potential consequences of different allocation strategies for patients, donors, and society as a whole?\n7. How can ethical theories such as utilitarianism, deontology, and virtue ethics inform the committee's decision-making?\n8. What are the broader ethical implications of organ transplant allocation for healthcare policy, resource allocation, and social justice?""
}";
        public static string ProjectBasedLearning = @"Example of a high school project-based learning activity on sustainable agriculture:
{
  ""Assignment"": ""Designing a sustainable agriculture project to address food security and environmental sustainability.\n\nProject Overview:\nYour task is to design a sustainable agriculture project for a community facing food insecurity and environmental degradation. This project should promote sustainable farming practices, enhance food production, and improve access to nutritious food.\n\nProject Tasks:\n1. Research different sustainable agriculture techniques, such as organic farming, permaculture, agroforestry, and hydroponics.\n2. Identify the food security challenges facing your chosen community, including issues of access, affordability, and nutritional quality.\n3. Develop a plan for implementing sustainable agriculture practices, including crop selection, soil management, water conservation, and pest control.\n4. Consider the social, economic, and environmental benefits of your project, as well as any potential challenges or obstacles.\n5. Implement your sustainable agriculture project, working collaboratively with community members, local organizations, and agricultural experts.\n6. Monitor and evaluate the impact of your project on food security, environmental sustainability, and community well-being.\n7. Reflect on the ethical considerations involved in sustainable agriculture, including issues of equity, justice, and environmental stewardship.\n\nProject Deliverables:\n- Project proposal outlining your sustainable agriculture project\n- Implementation plan detailing your project goals, activities, and timeline\n- Evaluation report assessing the impact of your project\n- Presentation to the community highlighting your project's achievements and lessons learned\n- Reflection essay on the ethical implications of sustainable agriculture and your role as a responsible global citizen.""
}
Example of a college level project-based learning activity on software development:
{
  ""Assignment"": ""Designing and design a web application to address a specific user need.\n\nProject Overview:\nYour task is to design a web application that solves a real-world problem or fulfills a specific user need. This project will involve the entire software designing lifecycle, from project planning and requirements gathering to design, simulation-testing, and presentation of the results.\n\nProject Tasks:\n1. Identify a problem or user need that can be addressed with a web application.\n2. Conduct user research to understand the target audience, their needs, and pain points.\n3. Define project requirements, features, and functionality based on user feedback and stakeholder input.\n4. Design the user interface (UI) and user experience (UX) of your web application, including wireframes, mockups, and user flow diagrams.\n5. Select appropriate technologies and tools for building your web application, such as HTML, CSS, JavaScript, and a backend framework like Node.js, Django, or Ruby on Rails.\n6. Test your prototyped web application to ensure that it meets user requirements, is free of errors, and provides a seamless user experience.\n8. Gather feedback from users and stakeholders, and iterate on your web application based on their input.\n9 Create a presentation of your final design to and expose it like you were looking for fundings. n\nProject Deliverables:\n- Project proposal outlining your web application idea, user research, and project requirements\n- UI/UX design documents, including wireframes, mockups, and user flow diagrams\n- Test cases and test results\n- Presentation of your final designed product.""
}";
        public static string ProblemSolvingActivity = @"Example of an academic level problem-solving activity on renewable energy:
{
  ""Assignment"": ""Work in teams to solve a renewable energy challenge involving the design of a solar power system.\n\nProblem Scenario:\nYour team has been tasked with designing a solar power system for a remote community that lacks access to reliable electricity. The system should be cost-effective, environmentally friendly, and capable of meeting the community's energy needs.\n\nProblem-Solving Tasks:\n1. Conduct a site assessment to determine the community's energy requirements and solar potential.\n2. Design a solar power system that meets the community's energy needs, taking into account factors such as available sunlight, energy demand, and system efficiency.\n3. Calculate the costs and benefits of your proposed solar power system, including installation, maintenance, and long-term energy savings.\n4. Develop a plan for implementing and managing the solar power system, including financing, training, and community engagement.\n5. Present your solar power system design to the class, explaining your approach, key design decisions, and expected outcomes.\n6. Reflect on the ethical considerations involved in renewable energy development, including issues of access, equity, and environmental justice.\n\nProblem-Solving Deliverables:\n- Solar power system design proposal\n- Cost-benefit analysis of your proposed system\n- Implementation plan outlining the steps for installing and managing the system\n- Presentation to the class\n- Reflection essay on the ethical implications of renewable energy development and the importance of sustainable energy solutions.""
}
Example of a middle school level problem-solving activity on math:
{
  ""Assignment"": ""Activity Overview:\nLearners will work individually or in small groups to solve a math problem involving fractions, proportions, and critical thinking skills. The goal is to determine how to fairly divide pizzas among a group of friends with different preferences.\n\nActivity Task:\n1. Problem scenario: A group of 12 friends is having a pizza party. They have ordered 5 large pizzas with different toppings: pepperoni, sausage, mushrooms, olives, and peppers. Each pizza has 8 slices. Each friend has different preferences for toppings, 2 frineds only like pepperoni, 2 only like mushrooms, 1 only likes olives, 1 only likes sausage, 3 like all the toppings and the other 3 like peppers, mushrooms and olives. They want to ensure that everyone gets a share as fair as possible of their favorite toppings.\n2. Work independently or in small groups to come up with a solution. How can you divide the pizzas so that each friend gets share as fair as possible of their favorite toppings? \n3. Present your solution to the class and explain your reasoning. How did you decide to divide the pizzas? Did you use fractions, proportions, or another method? Discuss any alternative solutions or strategies.""
}";
        public static string Simulation = @"Example of a high school level simulation activity on international diplomacy:
{
  ""Assignment"": ""Participate in a simulation activity to simulate a meeting of the United Nations Security Council.\n\nSimulation Scenario:\nYou will take on the role of a representative from a member state of the United Nations Security Council. Your task is to address a crisis situation and negotiate a resolution with other council members.\n\nSimulation Tasks:\n1. Research the foreign policy objectives and priorities of the country you represent.\n2. Analyze the crisis situation and develop a negotiating strategy based on your country's interests and goals.\n3. Participate in simulated Security Council meetings, working with other representatives to draft and negotiate a resolution.\n4. Consider the ethical, political, and strategic implications of different policy options, as well as the need for international cooperation and collective security.\n5. Reach consensus on a resolution to address the crisis situation and maintain peace and security.\n6. Reflect on the challenges of international diplomacy and the role of ethics, justice, and human rights in global politics.\n\nSimulation Deliverables:\n- Country briefing document outlining your foreign policy objectives and priorities\n- Negotiating strategy and position papers\n- Security Council resolution drafted during the simulation\n- Reflection essay on the ethical dimensions of international diplomacy and the challenges of maintaining peace and security in a complex and interconnected world.""
}
Example of a middle school level simulation activity on a the French Revolution:
{
  ""Assignment"": ""Role-playing activity simulating the key events and debates of the French Revolution.\n\nSimulation Overview:\nIn this simulation activity, Learners will role-play as key figures during the French Revolution. The goal is to recreate the political and social dynamics of revolutionary France and understand the causes, events, and outcomes of this pivotal historical period.\n\nSimulation Tasks:\n1. Assign each student a role as a historical figure from the French Revolution, such as King Louis XVI, Marie Antoinette, Maximilien Robespierre, Georges Danton, or Jean-Paul Marat.\n2. Research the major events and issues of the French Revolution, including the political, social, and economic causes, as well as the key figures and factions involved.\n3. Participate in simulated debates and discussions on these issues, working with other historical figures to negotiate and compromise on key decisions.\n4. Role-play the major events of the French Revolution, such as the storming of the Bastille, the Reign of Terror, the rise of the Jacobins, and the execution of King Louis XVI.\n5. Reflect on the decision-making process and the challenges of revolution, reform, and political change.\n\nSimulation Roles:\n- King Louis XVI\n- Marie Antoinette\n- Maximilien Robespierre\n- Georges Danton\n- Jean-Paul Marat\n- Jacques Necker\n- Charlotte Corday\n- And other historical figures representing different factions and perspectives\n\nSimulation Outcome:\n- Gain a deeper understanding of the French Revolution and its significance in world history\n- Develop research, critical thinking, and negotiation skills through role-playing and debate\n- Reflect on the causes, events, and outcomes of the French Revolution and its impact on modern society\n- Gain insight into the complexities of political change, revolution, and social reform"";
}";
        public static string InquiryBasedLearning = @"Example of a college level inquiry-based learning activity on genetic engineering:
{
  ""Assignment"": ""Engage in an inquiry-based learning activity to explore the ethical implications of genetic engineering.\n\nInquiry Questions:\n1. What is genetic engineering, and how does it work?\n2. What are the potential benefits of genetic engineering in areas such as agriculture, medicine, and biotechnology?\n3. What are the ethical concerns raised by the use of genetic engineering, including issues of consent, safety, and environmental impact?\n4. How do different ethical theories and frameworks inform our understanding of the ethical implications of genetic engineering?\n5. What role should government regulation and oversight play in governing the use of genetic engineering?\n6. How can we balance the potential benefits of genetic engineering with the need to address ethical concerns and minimize risks?\n7. What are the long-term implications of genetic engineering for society, including issues of social justice, equity, and human enhancement?\n8. How can ethical reflection and dialogue inform responsible decision-making and public policy in the field of genetic engineering?\n\nInquiry-Based Learning Activities:\n- Research different applications of genetic engineering and their ethical implications\n- Analyze case studies and real-world examples of genetic engineering projects\n- Engage in group discussions and debates on ethical dilemmas in genetic engineering\n- Reflect on your own values, beliefs, and ethical principles in relation to genetic engineering\n- Collaborate on a final project that explores the ethical dimensions of genetic engineering and proposes recommendations for responsible innovation and regulation.\n\nInquiry-Based Learning Outcomes:\n- Develop a deeper understanding of the ethical implications of genetic engineering\n- Enhance critical thinking, research, and analytical skills\n- Foster ethical reflection, dialogue, and responsible decision-making\n- Gain insight into the complex relationship between science, technology, and ethics in contemporary society.""
}
Example of a high school level inquiry-based learning activity on artificial intelligence:
{
  ""Assignment"": ""Engage in an inquiry-based learning activity to explore the opportunities and challenges of artificial intelligence (AI).\n\nInquiry Questions:\n1. What is artificial intelligence, and how is it used in everyday life?\n2. What are the potential benefits of artificial intelligence in various fields, such as healthcare, education, transportation, and entertainment?\n3. What are the challenges and ethical considerations associated with the development and use of artificial intelligence?\n4. How does artificial intelligence impact society, including issues such as job displacement, privacy concerns, and bias in AI algorithms?\n5. What are the current trends and future directions of artificial intelligence technology?\n\nInquiry-Based Learning Activities:\n- Research real-world examples of artificial intelligence applications and their impact on society\n- Analyze case studies and news articles related to artificial intelligence\n- Engage in group discussions and debates on the ethical and societal implications of artificial intelligence\n- Interview professionals working in the field of artificial intelligence to gain insights into current trends and future developments\n- Collaborate on a final project that explores the opportunities and challenges of artificial intelligence and proposes recommendations for responsible AI development and deployment.\n\nInquiry-Based Learning Outcomes:\n- Develop a deeper understanding of artificial intelligence technology and its impact on society\n- Enhance critical thinking, research, and analytical skills\n- Foster ethical reflection, dialogue, and responsible decision-making\n- Gain insight into the role of artificial intelligence in shaping the future of technology and society.""
}";


        public static string LearningObjective = @"{
  ""BloomLevel"": 1,
  ""MacroSubject"": ""Literature"",
  ""Level"": 3,
  ""Topic"": ""Shakespeare's poem""
}

{
  ""BloomLevel"": 2,
  ""MacroSubject"": ""Mathematics"",
  ""Level"": 2,
  ""Topic"": ""Parabolic Motion""
}

{
  ""BloomLevel"": 5,
  ""MacroSubject"": ""History"",
  ""Level"": 3,
  ""Topic"": ""Pearl Harbour""
}

{
  ""BloomLevel"": 4,
  ""MacroSubject"": ""Educazione"",
  ""Level"": 4,
  ""Topic"": ""Tecniche di studio""
}

{
  ""BloomLevel"": 3,
  ""MacroSubject"": ""Chemistry"",
  ""Level"": 2,
  ""Topic"": ""Redox Reaction""
}

{
  ""BloomLevel"": 1,
  ""MacroSubject"": ""Física"",
  ""Level"": 2,
  ""Topic"": ""Teoría de la relatividad""
}

{
  ""BloomLevel"": 3,
  ""MacroSubject"": ""Sciences de la Terre"",
  ""Level"": 3,
  ""Topic"": ""Déforestation en Amazonie""
}";
        public static string LearningObjectives = @"
Example in English:""{
  ""Remembering"":[
    ""Learners will be able to recall the definition of uniform acceleration motion."",
    ""Learners will be able to list the equations that describe uniform acceleration motion.""
  ],
  ""Understanding"":[
    ""Learners will be able to explain the concept of uniform acceleration motion in your own words."",
    ""Learners will be able to interpret graphs depicting uniform acceleration motion.""
  ],
  ""Applying"":[
    ""Learners will be able to solve problems involving uniform acceleration motion using the appropriate equations."",
    ""Learners will be able to design an experiment to measure the acceleration of an object in uniform motion.""
  ],
  ""Analyzing"":[
    ""Learners will be able to compare and contrast uniform acceleration motion with il moto uniforme."",
    ""Learners will be able to analyze real-life examples of uniform acceleration motion and identify the factors affecting acceleration.""
  ],
  ""Evaluating"":[
    ""Learners will be able to critique the validity of experimental procedures used to measure acceleration in various scenarios."",
    ""Learners will be able to evaluate the efficiency of different methods for calculating acceleration in uniform acceleration motion.""
  ],
  ""Creating"":[
    ""Learners will be able to develop a scenario involving uniform acceleration motion and solve related problems."",
    ""Learners will be able to construct a model or simulation to demonstrate uniform acceleration motion.""
  ]
}""

Example in Italian:""{
  ""Remembering"":[
    ""Gli studenti saranno in grado di ricordare la definizione di il moto uniformemente accelerato."",
    ""Gli studenti saranno in grado di ricordare le equazioni che descrivono il moto uniformemente accelerato.""
  ],
  ""Understanding"":[
    ""Gli studenti saranno in grado di spiegare il concetto di il moto uniformemente accelerato con parole proprie."",
    ""Gli studenti saranno in grado di interpretare i grafici che rappresentano il moto uniformemente accelerato.""
  ],
  ""Applying"":[
    ""Gli studenti saranno in grado di risolvere problemi che coinvolgono il moto uniformemente accelerato utilizzando le equazioni appropriate."",
    ""Gli studenti saranno in grado di progettare un esperimento per misurare l'accelerazione di un oggetto in moto uniformemente accelerato.""
  ],
  ""Analyzing"":[
    ""Gli studenti saranno in grado di confrontare e mettere in contrasto il moto uniformemente accelerato con il moto uniforme."",
    ""Gli studenti saranno in grado di analizzare esempi della vita reale di moto uniformemente accelerato e identificare i fattori che influenzano l'accelerazione.""
  ],
  ""Evaluating"":[
    ""Gli studenti saranno in grado di criticare la validità delle procedure sperimentali utilizzate per misurare l'accelerazione in diversi scenari."",
    ""Gli studenti saranno in grado di valutare l'efficienza di diversi metodi per calcolare l'accelerazione nel moto uniformemente accelerato.""
  ],
  ""Creating"":[
    ""Gli studenti saranno in grado di sviluppare uno scenario che coinvolge il moto uniformemente accelerato e risolvere problemi correlati."",
    ""Gli studenti saranno in grado di costruire un modello o una simulazione per dimostrare il moto uniformemente accelerato.""
  ]
}""

Example in French:""{
  ""Remembering"":[
    ""Les apprenants seront capables de se souvenir de la définition de il moto uniformemente accelerato."",
    ""Les apprenants seront capables de énumérer les équations qui décrivent il moto uniformemente accelerato.""
  ],
  ""Understanding"":[
    ""Les apprenants seront capables de expliquer le concept de il moto uniformemente accelerato avec ses propres mots."",
    ""Les apprenants seront capables de interpréter les graphiques représentant il moto uniformemente accelerato.""
  ],
  ""Applying"":[
    ""Les apprenants seront capables de résoudre des problèmes impliquant il moto uniformemente accelerato en utilisant les équations appropriées."",
    ""Les apprenants seront capables de concevoir une expérience pour mesurer l'accélération d'un objet en mouvement uniformément accéléré.""
  ],
  ""Analyzing"":[
    ""Les apprenants seront capables de comparer et mettre en contraste il moto uniformemente accelerato avec il moto uniforme."",
    ""Les apprenants seront capables de analyser des exemples de la vie réelle de il moto uniformemente accelerato et identifier les facteurs affectant l'accélération.""
  ],
  ""Evaluating"":[
    ""Les apprenants seront capables de critiquer la validité des procédures expérimentales utilisées pour mesurer l'accélération dans différents scénarios."",
    ""Les apprenants seront capables de évaluer l'efficacité de différentes méthodes pour calculer l'accélération dans il moto uniformemente accelerato.""
  ],
  ""Creating"":[
    ""Les apprenants seront capables de développer un scénario impliquant il moto uniformemente accelerato et résoudre des problèmes associés."",
    ""Les apprenants seront capables de construire un modèle ou une simulation pour démontrer il moto uniformemente accelerato.""
  ]
}""

Example in German:""{
  ""Remembering"":[
    ""Lernende werden in der Lage sein zu erinnern Sie sich an die Definition von il moto uniformemente accelerato."",
    ""Lernende werden in der Lage sein zu auflisten der Gleichungen, die il moto uniformemente accelerato beschreiben.""
  ],
  ""Understanding"":[
    ""Lernende werden in der Lage sein zu erklären Sie das Konzept von il moto uniformemente accelerato in eigenen Worten."",
    ""Lernende werden in der Lage sein zu interpretieren von Grafiken, die il moto uniformemente accelerato darstellen.""
  ],
  ""Applying"":[
    ""Lernende werden in der Lage sein zu lösen von Problemen, die il moto uniformemente accelerato unter Verwendung der entsprechenden Gleichungen betreffen."",
    ""Lernende werden in der Lage sein zu entwerfen eines Experiments zur Messung der Beschleunigung eines Objekts in gleichmäßiger Bewegung.""
  ],
  ""Analyzing"":[
    ""Lernende werden in der Lage sein zu vergleichen und kontrastieren Sie il moto uniformemente accelerato mit il moto uniforme."",
    ""Lernende werden in der Lage sein zu analysieren von Beispielen aus dem wirklichen Leben von il moto uniformemente accelerato und Identifizieren der Faktoren, die die Beschleunigung beeinflussen.""
  ],
  ""Evaluating"":[
    ""Lernende werden in der Lage sein zu kritisieren Sie die Gültigkeit experimenteller Verfahren zur Messung der Beschleunigung in verschiedenen Szenarien."",
    ""Lernende werden in der Lage sein zu bewerten Sie die Effizienz verschiedener Methoden zur Berechnung der Beschleunigung in il moto uniformemente accelerato.""
  ],
  ""Creating"":[
    ""Lernende werden in der Lage sein zu entwickeln Sie ein Szenario, das il moto uniformemente accelerato einbezieht, und lösen Sie damit verbundene Probleme."",
    ""Lernende werden in der Lage sein zu erstellen Sie ein Modell oder eine Simulation, um il moto uniformemente accelerato zu demonstrieren.""
  ]
}""

Example in Spanish:""{
  ""Remembering"":[
    ""Los aprendices serán capaces de recordar la definición de il moto uniformemente accelerato."",
    ""Los aprendices serán capaces de recordar enumerar las ecuaciones que describen il moto uniformemente accelerato.""
  ],
  ""Understanding"":[
    ""Los aprendices serán capaces de recordar explicar el concepto de il moto uniformemente accelerato con sus propias palabras."",
    ""Los aprendices serán capaces de recordar interpretar gráficos que representen il moto uniformemente accelerato.""
  ],
  ""Applying"":[
    ""Los aprendices serán capaces de recordar resolver problemas que involucren il moto uniformemente accelerato utilizando las ecuaciones apropiadas."",
    ""Los aprendices serán capaces de recordar diseñar un experimento para medir la aceleración de un objeto en movimiento uniformemente acelerado.""
  ],
  ""Analyzing"":[
    ""Los aprendices serán capaces de recordar comparar y contrastar il moto uniformemente accelerato con il moto uniforme."",
    ""Los aprendices serán capaces de recordar analizar ejemplos de la vida real de il moto uniformemente accelerato e identificar los factores que afectan la aceleración.""
  ],
  ""Evaluating"":[
    ""Los aprendices serán capaces de recordar critique la validez de los procedimientos experimentales utilizados para medir la aceleración en diferentes escenarios."",
    ""Los aprendices serán capaces de recordar evaluar la eficacia de diferentes métodos para calcular la aceleración en il moto uniformemente accelerato.""
  ],
  ""Creating"":[
    ""Los aprendices serán capaces de recordar desarrollar un escenario que involucre il moto uniformemente accelerato y resolver problemas relacionados."",
    ""Los aprendices serán capaces de recordar construir un modelo o simulación para demostrar il moto uniformemente accelerato.""
  ]
}""";

        public static string SyllabusExamples = @"""{
  ""CourseTitle"": ""Introduction to Organic Chemistry"",
  ""CourseDescription"": ""This college-level course introduces students to the fundamental principles of organic chemistry. Topics include the structure, properties, and reactions of organic molecules, with a focus on understanding the mechanisms that govern chemical behavior. Laboratory sessions complement the theoretical knowledge gained in lectures."",
  ""LearningOutcomes"": [
    ""Understand the structure and bonding of organic molecules"",
    ""Predict the reactivity of organic compounds based on functional groups"",
    ""Analyze and interpret spectroscopic data to identify organic substances"",
    ""Apply organic reaction mechanisms to synthesize target compounds"",
    ...,
    ""Demonstrate safe and effective laboratory techniques in organic chemistry experiments""
  ],
  ""CourseGoals"": [
    ""Provide a solid foundation in organic chemistry principles"",
    ""Prepare students for advanced chemistry courses and research"",
    ""Develop problem-solving skills in the context of chemical reactions"",
    ...,
    ""Promote a hands-on understanding of organic synthesis through laboratory work""
  ],
  ""CourseTopics"": [
    {
      ""Topic"": ""Structure and Bonding"",
      ""Description"": ""Introduction to atomic structure, molecular orbitals, and the types of bonds in organic molecules.""
    },
    {
      ""Topic"": ""Functional Groups and Reactivity"",
      ""Description"": ""Study of common organic functional groups and their influence on molecular reactivity.""
    },
    {
      ""Topic"": ""Reaction Mechanisms"",
      ""Description"": ""Detailed exploration of reaction types such as substitution, elimination, and addition, including their mechanisms.""
    },
    {
      ""Topic"": ""Spectroscopy and Structure Determination"",
      ""Description"": ""Use of IR, NMR, and Mass Spectrometry to determine the structure of organic molecules.""
    },
    {
      ""Topic"": ""Organic Synthesis"",
      ""Description"": ""Application of reaction mechanisms in the synthesis of complex organic compounds.""
    }
  ],
  ""Prerequisites"": [
    ""Completion of General Chemistry I and II"",
    ""Basic understanding of chemical bonding and reactions"",
    ...,
    ""Familiarity with laboratory safety procedures and equipment""
  ]
}

{
  ""CourseTitle"": ""The Second World War: A Comprehensive Study"",
  ""CourseDescription"": ""This high school course offers an in-depth analysis of the Second World War, covering its causes, major events, and global impact. Students will explore the political, social, and economic factors that shaped the war and examine its long-term consequences on the modern world."",
  ""LearningOutcomes"": [
    ""Identify the key causes and events leading up to the Second World War"",
    ""Remember the strategies and outcomes of major battles and campaigns"",
    ""Understand the social and economic impact of the war on different countries"",
    ...,
    ""Understand the role of major world leaders and their decisions during the war""
  ],
  ""CourseGoals"": [
    ""Develop a comprehensive understanding of the Second World War"",
    ""Develop a detailed understanding about historical events and their causes"",
    ...,
    ""Encourage students to explore the human impact of global conflicts"",
    ""Prepare students for advanced history courses""
  ],
  ""CourseTopics"": [
    {
      ""Topic"": ""Causes of the Second World War"",
      ""Description"": ""Examination of the political and economic conditions that led to the outbreak of the war.""
    },
    {
      ""Topic"": ""Major Battles and Campaigns"",
      ""Description"": ""Detailed analysis of key battles, including Stalingrad, D-Day, and the Pacific Theater.""
    },
    {
      ""Topic"": ""The Home Fronts"",
      ""Description"": ""Study of the impact of the war on civilian life, including rationing, propaganda, and war production.""
    },
    {
      ""Topic"": ""The Holocaust and War Crimes"",
      ""Description"": ""Exploration of the Holocaust, genocide, and the war crimes committed during the conflict.""
    },
    {
      ""Topic"": ""Post-War Consequences"",
      ""Description"": ""Discussion of the outcomes of the war, including the establishment of the United Nations and the Cold War.""
    }
  ],
  ""Prerequisites"": [
    ""None; this course is open to all high school students""
  ]
}


{
  ""CourseTitle"": ""Advanced Prompt Engineering for AI Systems"",
  ""CourseDescription"": ""This course provides an in-depth exploration of prompt engineering techniques used in AI systems. Students will learn how to design, optimize, and evaluate prompts to improve the performance of language models in various applications. The course covers key concepts such as prompt formulation, bias mitigation, and prompt tuning across different contexts."",
  ""LearningOutcomes"": [
    ""Remember the best practices in prompt generation for AI systems"",
    ""Understand the impact of different prompt structures on model outputs"",
    ...,
    ""Remember the major techniques used to minimize biases and improve fairness in prompts""
  ],
  ""CourseGoals"": [
    ""Teach students some advanced skills in prompt engineering"",
    ""Prepare students for laboratory sessions where they will design and evaluate prompts"",
    ...,
    ""Give a general overview about the ethical implications of AI prompts""
  ],
  ""CourseTopics"": [
    {
      ""Topic"": ""Introduction to Prompt Engineering"",
      ""Description"": ""Overview of prompt engineering, its significance in AI, and the foundational principles.""
    },
    {
      ""Topic"": ""Prompt Formulation and Structure"",
      ""Description"": ""Detailed exploration of how to construct prompts and the impact of different structures on AI performance.""
    },
    {
      ""Topic"": ""Bias Mitigation in Prompts"",
      ""Description"": ""Techniques and strategies for identifying and reducing biases in AI prompts.""
    },
    {
      ""Topic"": ""Advanced Prompt Optimization"",
      ""Description"": ""Methods for fine-tuning and optimizing prompts to achieve specific outcomes.""
    },
    {
      ""Topic"": ""Case Studies and Applications"",
      ""Description"": ""Real-world applications of prompt engineering in various domains such as healthcare, finance, and education.""
    }
  ],
  ""Prerequisites"": [
    ""Basic understanding of AI and machine learning concepts"",
    ""Experience with Python programming"",
    ...,
    ""Completion of introductory courses in natural language processing""
  ]
}""";
        public static string ExerciseCorrections = @"
Question: What is quantum entanglement, and how does it impact the state description of particles?
Answer: Quantum entanglement is a quantum mechanical phenomenon where particles, even when separated by distance, become interdependent in a manner that the state of one particle is inseparable from the state of another. 
{
""Accuracy"": 0.8, 
""Correction"": null
}

Question: Secondo le leggi della termodinamica, quale principio afferma che l'energia non può essere creata né distrutta, ma solo trasformata da una forma all'altra?
Answer: La prima legge della termodinamica, ovvero la 'legge di conservazione dell'energia'.
{
""Accuracy"": 1.0,
""Correction"": ""null""
}

Question: Enuncia le leggi della termodinamica.
Answer: Prima legge della termodinamica (Conservazione dell'energia): L'energia non può essere creata né distrutta, ma solo trasformata da una forma all'altra. In altre parole, l'energia totale di un sistema isolato rimane costante nel tempo. Seconda legge della termodinamica (Legge dell'entropia): L'entropia di un sistema isolato tende ad aumentare nel tempo. Questa legge stabilisce che il disordine di un sistema isolato aumenta nel corso di una trasformazione spontanea. Zeroth law of thermodynamics (Legge zero della termodinamica): Se due sistemi sono in equilibrio termico con un terzo sistema, allora sono in equilibrio termico tra loro.
{
""Accuracy"": 0.5,
""Correction"": ""La risposta data è corretta, ma non è stata enunciata la Terza legge della termodinamica (Teorema di Nernst): Questa legge stabilisce che è impossibile raggiungere il valore di zero assoluto in un numero finito di passaggi termodinamici. Consiglio di ristudiare questa terza legge e vedere alcuna applicazioni pratiche per aiutare a ricordarla""
}

Question: Qu'est-ce que l'entrelacement quantique, et comment cela impacte-t-il la description de l'état des particules?
Answer: L'entrelacement quantique est un phénomène rare qui s'applique uniquement aux particules en laboratoire.
{
""Accuracy"": 0.0,
""Correction"": ""La réponse fournie est incorrecte car elle affirme que l'entrelacement quantique est un phénomène rare qui ne s'applique qu'aux particules en laboratoire. Ceci est incorrect car l'entrelacement quantique est un aspect fondamental de la mécanique quantique et a été démontré dans de nombreuses expériences impliquant un large éventail de particules et de conditions. Il n'est pas limité aux paramètres de laboratoire et est un phénomène bien établi dans le domaine de la physique quantique. Pour améliorer la compréhension dans ce domaine, il serait bénéfique d'étudier le principe de l'entrelacement quantique et les expériences qui l'ont démontré dans diverses conditions et avec différents types de particules.""
}

Question: Was ist Quantenverschränkung, und wie beeinflusst sie die Zustandsbeschreibung von Teilchen?
Answer: Quantenverschränkung ist eine einfache Interaktion zwischen Teilchen, die über eine Entfernung hinweg ohne Einfluss auf ihre individuellen Zustände stattfindet.
{
""Accuracy"": 0.4,
""Correction"": ""Die gegebene Antwort ist inkorrekt, da sie das Konzept der Quantenverschränkung über vereinfacht und ihre Auswirkungen auf die Zustandsbeschreibung von Teilchen falsch darstellt. Quantenverschränkung ist keine einfache Interaktion zwischen Teilchen über eine Entfernung hinweg; es handelt sich um ein komplexes Phänomen, bei dem die Quantenzustände von zwei oder mehr Teilchen miteinander verbunden werden. Sobald diese Teilchen verschränkt sind, kann der Zustand eines Teilchens nicht unabhängig von dem/den anderen beschrieben werden, egal wie weit sie voneinander entfernt sind. Dies bedeutet, dass eine Änderung des Zustands eines Teilchens sofort den Zustand des anderen Teilchens beeinflusst, unabhängig von der Entfernung zwischen ihnen. Dies ist ein grundlegendes Merkmal der Quantenmechanik und hat bedeutende Auswirkungen auf unser Verständnis der Natur auf ihrer fundamentalsten Ebene.Um das Verständnis in diesem Bereich zu verbessern, wäre es vorteilhaft, das Prinzip der quantenmechanischen Verschränkung und die Experimente, die es unter verschiedenen Bedingungen und Teilchentypen demonstriert haben, zu studieren.""
}";

        public static string MaterialAnalysisExamples = @"
{
  ""Language"": ""English"",
  ""MacroSubject"": ""History"",
  ""Title"": ""The American Civil War: Causes, Key Battles, and the Emancipation Proclamation"",
  ""PerceivedDifficulty"": ""high_school"",
  ""MainTopics"": [
    {
      ""Topic"": ""Causes of the Civil War"",
      ""Description"": ""Explanation of the economic, social, and political factors that led to the conflict."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Understanding"",
      ""Start"": ""The first factor that contributed"",
      ""Keywords"": [""economic factors"", ""social factors"", ""political factors"", ""conflict""]
    },
    {
      ""Topic"": ""Major Battles"",
      ""Description"": ""Overview of key battles and their significance."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Remembering"",
      ""Start"": ""In 1861 the first major"",
      ""Keywords"": [""key battles"", ""significance"", ""military history""]
    },
    {
      ""Topic"": ""Emancipation Proclamation"",
      ""Description"": ""Analysis of Lincoln's executive order and its impact."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Analyzing"",
      ""Start"": ""The Emancipation Proclamation was issued"",
      ""Keywords"": [""Emancipation Proclamation"", ""Lincoln"", ""executive order"", ""impact""]
    }
  ]
}

{
  ""Language"": ""Italian"",
  ""MacroSubject"": ""History"",
  ""Title"": ""Interesting Facts About Genghis Khan: Strategic Marriages, Secret Tomb, and His Descendants"",
  ""PerceivedDifficulty"": ""high_school"",
  ""MainTopics"": [
    {
      ""Topic"": ""Strategic Marriages"",
      ""Description"": ""Genghis Khan arranged strategic marriages for his daughters, often to allied rulers, and then assigned military missions to his sons-in-law."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Understanding"",
      ""Start"": ""In the 13th century, Genghis "",
      ""Keywords"": [""strategic marriages"", ""daughters"", ""allied rulers"", ""military missions""]
    },
    {
      ""Topic"": ""Secret Tomb"",
      ""Description"": ""Genghis Khan's tomb has never been found, as he ordered it to remain a secret. Thousands of people who attended his funeral were executed to keep the location hidden."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Evaluating"",
      ""Start"": ""The location of Genghis Khan's"",
      ""Keywords"": [""secret tomb"", ""Genghis Khan"", ""funeral"", ""hidden location""]
    },
    {
      ""Topic"": ""Descendants"",
      ""Description"": ""Genghis Khan had so many children and wives that 1 in 200 people today are believed to be his descendants."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Applying"",
      ""Start"": ""An interesting fact about Genghis"",
      ""Keywords"": [""descendants"", ""Genghis Khan"", ""children"", ""wives"", ""genetics""]
    }
  ]
}


{
  ""Language"": ""Spanish"",
  ""MacroSubject"": ""Literature"",
  ""Title"": ""Don Quixote: Characters, Themes, and Narrative Style"",
  ""PerceivedDifficulty"": ""college"",
  ""MainTopics"": [
    {
      ""Topic"": ""Characters"",
      ""Description"": ""Description of Don Quixote, Sancho Panza, and other key characters."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Remembering"",
      ""Start"": ""The main characters in Don"",
      ""Keywords"": [""Don Quixote"", ""Sancho Panza"", ""key characters""]
    },
    {
      ""Topic"": ""Themes"",
      ""Description"": ""Exploration of themes such as chivalry, reality vs. illusion, and madness."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Understanding"",
      ""Start"": ""At that time chivalry was"",
      ""Keywords"": [""themes"", ""chivalry"", ""reality"", ""illusion"", ""madness""]
    },
    {
      ""Topic"": ""Narrative Style"",
      ""Description"": ""Analysis of Cervantes' narrative techniques and metafictional elements."",
      ""Type"": ""Theoretical"",
      ""Bloom"": ""Creating"",
      ""Start"": ""The narrative style of Don"",
      ""Keywords"": [""narrative style"", ""Cervantes"", ""techniques"", ""metafiction""]
    }
  ]
}";
    
        public static string LessonPlanExamples = @"
""{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Introduction to the rise of the Napoleon empire"",
      ""description"": ""Use a slide presentation to introduce the Learners to the lesson topics by providing an overview of the lesson plan."",
      ""duration"": 10
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Context of the historical situation before the rise of the Napoleon empire"",
      ""description"": ""group_discussion"",
      ""duration"": 15
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Timeline of key events"",
      ""description"": ""Create a visually engaging timeline of key events leading to the rise of the Napoleon empire."",
      ""duration"": 20
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""The rise of Napoleon, the two coups d'état"",
      ""description"": ""simulation"",
      ""duration"": 25
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""The rise of the Napoleon empire"",
      ""description"": ""multiple_choice"",
      ""duration"": 10
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Assessment: perspective of a common citizen living during the rise of the Napoleon empire"",
      ""description"": ""essay"",
      ""duration"": 40
    },
    
  ]
}
{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Introduction to Linear Algebra"",
      ""description"": ""Provide an overview of the basic concepts of linear algebra, including vectors, matrices, and linear transformations."",
      ""duration"": 15
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Matrix Operations"",
      ""description"": ""Demonstrate basic matrix operations such as addition, subtraction, scalar multiplication, and matrix multiplication."",
      ""duration"": 20
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Systems of Linear Equations"",
      ""description"": ""Using real life cases, introduce the concept of systems of linear equations and methods for solving them."",
      ""duration"": 25
    },
    {
      ""type"": ""Activiy"",
      ""topic"": ""Systems of Linear Equations"",
      ""description"": ""open_question"",
      ""duration"": 15
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Vector Spaces"",
      ""description"": ""Explore the properties of vector spaces, including basis, dimension, and linear independence."",
      ""duration"": 30
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Assessment: vector spaces and linear transformations"",
      ""description"": ""short_answer_question"",
      ""duration"": 25
    }
  ]
}
{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Introduction to Parabolic Motion"",
      ""description"": ""Introduce the concept of parabolic motion using videos and real-life examples."",
      ""duration"": 20
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Experiment on Projectile Motion"",
      ""description"": ""Perform an experiment to investigate the factors that affect the range and height of a projectile's trajectory."",
      ""duration"": 30
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Mathematical Modeling of Parabolic Motion"",
      ""description"": ""Teach Learners how to create mathematical models to describe parabolic motion using equations and graphs."",
      ""duration"": 15
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Analyzing Real-life Data"",
      ""description"": ""non_written_material_analysis"",
      ""duration"": 30
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Significance of parabolic motion in various fields, including physics, engineering, and mathematics."",
      ""description"": ""group_discussion"",
      ""duration"": 15
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Assessment: Parabolic Motion Problem"",
      ""description"": ""open_question"",
      ""duration"": 15
    }
  ]
}""";
        public static string CustomPlanExamples = @"
""{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Timeline of key events"",
      ""description"": ""Review the key events that led to the rise of the Napoleon empire."",
      ""duration"": 15
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Timeline of key events"",
      ""description"": ""multiple_choice"",
      ""duration"": 5
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""The rise of Napoleon, the two coups d'état"",
      ""description"": ""review the dynamics of the two coups d'état that brought Napoleon to power."",
      ""duration"": 20
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""The rise of the Napoleon empire"",
      ""description"": ""open_question"",
      ""duration"": 10
    }
  ]
}
{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Matrix Operations"",
      ""description"": ""Review matrix addition, scalar multiplication, and matrix multiplication."",
      ""duration"": 20
    },
    {
      ""type"": ""Activiy"",
      ""topic"": ""Matrix Operations"",
      ""description"": ""open_question"",
      ""duration"": 15
    }
  ]
}
{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Mathematical Modeling of Projectile Motion"",
      ""description"": ""Review mathematical models and formulas used to describe projectile motion."",
      ""duration"": 15
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Mathematical Modeling of Projectile Motion"",
      ""description"": ""multiple_choice"",
      ""duration"": 10
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Mathematical Modeling of Projectile Motion"",
      ""description"": ""short_answer_question"",
      ""duration"": 20
    }
  ]
}
{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Photosynthesis"",
      ""description"": ""Review the process of photosynthesis and its significance in the plant kingdom."",
      ""duration"": 15
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Photosynthesis"",
      ""description"": ""true_or_false"",
      ""duration"": 15
    }
  ]
}
{
  ""lesson_plan"": [
    {
      ""type"": ""Lesson"",
      ""topic"": ""Mesozoic Era"",
      ""description"": ""Review the characteristics and major events of the Mesozoic Era."",
      ""duration"": 15
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Mesozoic Era"",
      ""description"": ""information_search"",
      ""duration"": 10
    },
    {
      ""type"": ""Lesson"",
      ""topic"": ""Dinosaurs"",
      ""description"": ""Review the classification, anatomy, and behavior of dinosaurs."",
      ""duration"": 25
    },
    {
      ""type"": ""Activity"",
      ""topic"": ""Dinosaurs"",
      ""description"": ""multiple_select"",
      ""duration"": 10
    }
  ]
}""";
        public static string CoursePlanExamples = @"
""{
  ""plan"": [
    {
      ""title"": ""Gli Alleati prendono l'iniziativa: le campagne di riconquista"",
      ""topics"": [
        ""La battaglia di Stalingrado"",
        ""La conferenza di Casablanca"",
        ""La campagna del Nord Africa"",
        ""La conquista di Sicilia e Corsica""
      ]
    },
    {
      ""title"": ""La svolta finale della guerra"",
      ""topics"": [
        ""Lo sbarco in Normandia (D-Day)"",
        ""La liberazione di Parigi"",
        ""La resistenza europea"",
        ""La conferenza di Yalta""
      ]
    },
    {
      ""title"": ""La conclusione della guerra e le sue conseguenze"",
      ""topics"": [
        ""La battaglia di Berlino"",
        ""La resa della Germania nazista"",
        ""L'esplosione delle bombe atomiche su Hiroshima e Nagasaki"",
        ""La fine della seconda guerra mondiale"",
        ""Le conferenze di Teheran e Potsdam"",
        ""La nascita delle Nazioni Unite""
      ]
    }
  ]
}
{
  ""plan"": [
    {
      ""title"": ""Los Aliados toman la iniciativa: las campañas de reconquista"",
      ""topics"": [
        ""La batalla de Stalingrado"",
        ""La conferencia de Casablanca"",
        ""La campaña del Norte de África"",
        ""La conquista de Sicilia y Córcega""
      ]
    },
    {
      ""title"": ""El giro final de la guerra"",
      ""topics"": [
        ""El desembarco de Normandía (Día D)"",
        ""La liberación de París"",
        ""La resistencia europea"",
        ""La conferencia de Yalta""
      ]
    },
    {
      ""title"": ""La conclusión de la guerra y sus consecuencias"",
      ""topics"": [
        ""La batalla de Berlín"",
        ""La rendición de la Alemania nazi"",
        ""La explosión de las bombas atómicas en Hiroshima y Nagasaki"",
        ""El fin de la Segunda Guerra Mundial"",
        ""Las conferencias de Teherán y Potsdam"",
        ""El nacimiento de las Naciones Unidas""
      ]
    }
  ]
}""";
    }

    public class UtilsStrings{
        public static string ActivitiesList = @"
'open_question' (open question exercise that expects a free-form elaborated answer),
'short_answer_question' (open question exercise that expects a short exact answer),
'true_or_false' (true or false question, it may or may not require an explanation),
'information_search' (fill in the blanks exercise),
'multiple_choice' (multiple choice question with one correct answer),
'multiple_select' (multiple choice question with multiple correct answers),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),

'debate' (oral debate between groups of Learners),
'brainstorming' (group activity to generate ideas or solutions about a specifc topic),
'group_discussion' (group activity to discuss a specific topic),
'simulation' (role-playing activity to simulate a situation about a specific topic),
'inquiry_based_learning' (activity where Learners explore a topic through inquiry and research),

'non_written_material_analysis' (analysis of non-written material such as images, videos, or audio),
'non_written_material_production' (production of non-written material such as images, videos, or audio),
'case_study_analysis' (analysis of a specific case study),
'project_based_learning' (activity where Learners work on a project to develope a real-world project),
'problem_solving_activity' (activity where Learners solve a specific problem)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListRemembering = @"
'true_or_false' (true or false question, it may or may not require an explanation),
'information_search' (fill in the blanks exercise),
'multiple_choice' (multiple choice question with one correct answer),
'multiple_select' (multiple choice question with multiple correct answers),
'short_answer_question' (open question exercise that expects a short exact answer)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListUnderstanding = @"
'true_or_false' (true or false question, it may or may not require an explanation),
'information_search' (fill in the blanks exercise),
'multiple_choice' (multiple choice question with one correct answer),
'multiple_select' (multiple choice question with multiple correct answers),
'short_answer_question' (open question exercise that expects a short exact answer),
'open_question' (open question exercise that expects a free-form elaborated answer),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListApplying = @"
'true_or_false' (true or false question, it may or may not require an explanation),
'multiple_choice' (multiple choice question with one correct answer),
'multiple_select' (multiple choice question with multiple correct answers),
'short_answer_question' (open question exercise that expects a short exact answer),
'open_question' (open question exercise that expects a free-form elaborated answer),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),
'simulation' (role-playing activity to simulate a situation about a specific topic),
'problem_solving_activity' (activity where Learners solve a specific problem),
'debate' (oral debate between groups of Learners)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListAnalyzing = @"
'true_or_false' (true or false question, it may or may not require an explanation),
'information_search' (fill in the blanks exercise),
'multiple_choice' (multiple choice question with one correct answer),
'multiple_select' (multiple choice question with multiple correct answers),
'short_answer_question' (open question exercise that expects a short exact answer),
'open_question' (open question exercise that expects a free-form elaborated answer),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),
'simulation' (role-playing activity to simulate a situation about a specific topic),
'problem_solving_activity' (activity where Learners solve a specific problem),
'debate' (oral debate between groups of Learners),
'brainstorming' (group activity to generate ideas or solutions about a specifc topic),
'group_discussion' (group activity to discuss a specific topic),
'case_study_analysis' (analysis of a specific case study),
'project_based_learning' (activity where Learners work on a project to develope a real-world project),
'non_written_material_analysis' (analysis of non-written material such as images, videos, or audio),
'inquiry_based_learning' (activity where Learners explore a topic through inquiry and research)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListEvaluating = @"
'short_answer_question' (open question exercise that expects a short exact answer),
'open_question' (open question exercise that expects a free-form elaborated answer),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),
'simulation' (role-playing activity to simulate a situation about a specific topic),
'problem_solving_activity' (activity where Learners solve a specific problem),
'debate' (oral debate between groups of Learners),
'brainstorming' (group activity to generate ideas or solutions about a specifc topic),
'group_discussion' (group activity to discuss a specific topic),
'case_study_analysis' (analysis of a specific case study),
'project_based_learning' (activity where Learners work on a project to develope a real-world project),
'non_written_material_analysis' (analysis of non-written material such as images, videos, or audio),
'inquiry_based_learning' (activity where Learners explore a topic through inquiry and research)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListCreating = @"
'short_answer_question' (open question exercise that expects a short exact answer),
'open_question' (open question exercise that expects a free-form elaborated answer),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),
'simulation' (role-playing activity to simulate a situation about a specific topic),
'problem_solving_activity' (activity where Learners solve a specific problem),
'debate' (oral debate between groups of Learners),
'brainstorming' (group activity to generate ideas or solutions about a specifc topic),
'group_discussion' (group activity to discuss a specific topic),
'case_study_analysis' (analysis of a specific case study),
'project_based_learning' (activity where Learners work on a project to develope a real-world project),
'non_written_material_analysis' (analysis of non-written material such as images, videos, or audio),
'inquiry_based_learning' (activity where Learners explore a topic through inquiry and research),
'non_written_material_production' (production of non-written material such as images, videos, or audio)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
        public static string ActivitiesListB = @"
'open_question' (open question exercise that expects a free-form elaborated answer),
'short_answer_question' (open question exercise that expects a short exact answer),
'true_or_false' (true or false question, it may or may not require an explanation),
'information_search' (fill in the blanks exercise),
'multiple_choice' (multiple choice question with one correct answer),
'multiple_select' (multiple choice question with multiple correct answers),
'essay' (open ended assignment that expects a full essay as answer),
'knoledge_exposition' (presentation or dissertation of a specific topic),
'non_written_material_analysis' (analysis of non-written material such as images, videos, or audio),
'non_written_material_production' (production of non-written material such as images, videos, or audio),
'case_study_analysis' (analysis of a specific case study),
'project_based_learning' (activity where Learners work on a project to develope a real-world project),
'problem_solving_activity' (activity where Learners solve a specific problem)
Keep the names in lowercase and use underscores (_) to separate words exactly as shown, it's crucial for post processing of the answer.";
    
    }
}
