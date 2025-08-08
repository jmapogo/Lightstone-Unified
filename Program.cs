using System.Text;

namespace Lightstone.SentenceParserApp
{
    public class Program
    { 
        static void Main(string[] args)
        { 
            const int SENTENCE_MAX_LENGTH_LIMIT = 25;
            ISentenceValidator sentenceValidator = new SentenceValidator(SENTENCE_MAX_LENGTH_LIMIT);
            IWordReverser reverser = new WordReverser(sentenceValidator);

            while (true) // Loop unti "^C" is entered, this can always be removed
            {
                bool InputIsValid = true;

                Console.Write("How many test cases would you like to reverse? (^C to exit): ");
                int inputCount = 0;

                if (!int.TryParse(Console.ReadLine(), out inputCount))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Invalid test case count");
                    continue;
                }

                string[] UserInput = new string[inputCount];

                for (int i = 0; i < inputCount; i++)
                {
                    Console.Write($"Please input sentence [{i + 1}]: ");

                    string tmpInput = Console.ReadLine();
                    try
                    {
                        if (sentenceValidator.isValid(tmpInput)) //IsValid throws an Exception when input is wrong
                        {
                            UserInput[i] = tmpInput;
                        } 

                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        InputIsValid = false;
                        break;
                        // There is no point continuiing because it will ruin the output.
                        //It is frustrating in situations where the users opeted to enter a huge number
                        // of test cases.
                    }
                }
                if (InputIsValid)
                {
                    IWordReverseManager wordReverseManager = new WordReverseManager(reverser, UserInput);

                    StringBuilder formattedOutput = wordReverseManager.FormatUserInput();

                    Console.WriteLine();
                    Console.WriteLine("Your reversed sentence(s): ");
                    Console.WriteLine(formattedOutput.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

        } 
    }

    #region Interfaces
    public interface ISentenceValidator
    {
        bool isValid(string Sentence);
    }
    public interface IWordReverseManager
    {
        StringBuilder FormatUserInput();
    }
    public interface IWordReverser
    {
        string ReverseSentence(string Sentence);
    }
    #endregion

    #region Implementations

    public class SentenceValidator : ISentenceValidator
    {
        private readonly int _maxLimit = 0;
        public SentenceValidator(int maxLimit)
        {
            _maxLimit = maxLimit;
        }
        private SentenceValidator() { }
        public bool isValid(string Sentence)
        {
            if (
                Sentence is null
                || Sentence.Length == 0
                || Sentence.Length > _maxLimit
                )
                throw new ArgumentException($"Invalid Sentence length must be between 1 and {_maxLimit} inclusively");

            if (HasSpecialChars(Sentence))
                throw new ArgumentException("Sentence must not contain any spacial characters other than a space");


            if (Sentence.StartsWith(' '))
                throw new ArgumentException("Sentence must not start with a space");

            if (Sentence.IndexOf("  ") > -1) //Double Space or more. first instance
                throw new ArgumentException("There must be exactly one space character between each pair of consecutive words");


            return true;
        }

        private bool HasSpecialChars(string Sentence)
        {
            //a-z (97 - 122)
            int a = 97, z = 122;
            //A-Z (65 - 90)
            int A = 65, Z = 90;
            //space (32)
            int space = 32;

            var SentenceCharArray = Sentence.ToCharArray();

            bool IContainsSpecials = false;
            foreach (char character in SentenceCharArray)
            {
                int CharASCIICode = (int)character;

                if (CharASCIICode >= a && CharASCIICode <= z) //check in a-z
                    continue;
                else if (CharASCIICode >= A && CharASCIICode <= Z) //check in A-Z
                    continue;
                if (CharASCIICode == space) //check space
                    continue;
                else // Must contain something else other than [a-zA-Z( )]
                {
                    IContainsSpecials = true;
                    break; // no need to check further
                }
            }
            return IContainsSpecials;
        }
    }

    public class WordReverseManager : IWordReverseManager
    {
        private readonly IWordReverser _reverser;
        private readonly string[] _userInput;
        public WordReverseManager(IWordReverser reverser, string[] userInput)
        {
            _reverser = reverser;
            _userInput = userInput;
        }
        /// <summary>
        /// 
        /// </summary> 
        /// <returns> 
        ///     Case 1: ReversedUserInput[i]
        ///     Case 2: ReversedUserInput[i]
        ///     Case 3: ReversedUserInput[i]
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public StringBuilder FormatUserInput()
        {
            if (_reverser is null || _userInput is null || _userInput.Length == 0)
                throw new ArgumentException("Invalid User Input");

            StringBuilder formattedOuput = new StringBuilder();
            int caseCounter = 0;

            foreach (var sentence in _userInput)
            {
                string reversedSentence = _reverser.ReverseSentence(sentence);

                string formattedLine = $"Case {++caseCounter}: {reversedSentence}";

                formattedOuput.AppendLine(formattedLine);
            }

            return formattedOuput;
        }
    }

    public class WordReverser : IWordReverser
    {
        private readonly ISentenceValidator _sentenceValidator;

        public WordReverser(ISentenceValidator sentenceValidator)
        {
            _sentenceValidator = sentenceValidator;
        }
        public string ReverseSentence(string Sentence)
        {
            try
            {
                if (!_sentenceValidator.isValid(Sentence))
                    throw new ArgumentException("Invalid User Input");

                var words = Sentence.Split(' ');

                Array.Reverse(words); // array ref reversed, nothing returned

                string ReversedWord = string.Join(" ", words);

                return ReversedWord;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }

    #endregion
}

#region Tests

namespace Lightstone.SentenceParserAppTests
{
    using Lightstone.SentenceParserApp;
    using NUnit.Framework;

    [TestFixture]
    public class WordReverseManagerTests
    {
        const int SENTENCE_MAX_LENGTH_LIMIT = 25;

        [Test]
        public void FormatUserInput_3ValidInput_OutputTheFormatedSentences()
        {
            ISentenceValidator sentenceValidator = new SentenceValidator(SENTENCE_MAX_LENGTH_LIMIT);
            IWordReverser wordReverser = new WordReverser(sentenceValidator);

            string[] userInput = new string[] {
                "this is a test",
                "foobar",
                "all your base"
            };

            StringBuilder expectedOutput = new StringBuilder();
            expectedOutput.AppendLine("Case 1: test a is this");
            expectedOutput.AppendLine("Case 2: foobar");
            expectedOutput.AppendLine("Case 3: base your all");

            IWordReverseManager wordReverseManager = new WordReverseManager
            (
                reverser: wordReverser,
                userInput: userInput
            );

            //Act
            StringBuilder result = wordReverseManager.FormatUserInput();

            //Assert
            Assert.That(result.ToString(), Is.EqualTo(expectedOutput.ToString()));

        }
    }
    [TestFixture]
    public class SentenceValidatorIsolatedTests
    {
        ISentenceValidator sentenceValidator;
        const int SENTENCE_MAX_LENGTH_LIMIT = 25;

        [SetUp]
        public void Setup()
        {
            sentenceValidator = new SentenceValidator(SENTENCE_MAX_LENGTH_LIMIT);
        }
        [Test]
        [TestCase(null)] // null
        [TestCase("")] // zero
        [TestCase("word word word word word word word word word word word")] //over 25
        public void SentenceValidator_LengthCheck_FailsIfExceeds25SpacesAndCharactersCombined(string? words)
        {
            Assert.Throws<ArgumentException>(() => sentenceValidator.isValid(words));
        }
        /// <summary>
        /// A line will only consist of letters and space characters. 
        /// There will be exactly one space character between each pair of consecutive words.
        /// Spaces will not appear at the start or end of a line.
        /// </summary>
        /// <param name="words"></param>
        [Test]
        [TestCase("word1 word2 word3!")] // special characters
        [TestCase("word1       word3")] // too many spaces
        [TestCase(" word1 word2")] // space at the start of the sentence
        public void SentenceValidator_DataCheck_FailsIfDoesNoAdhereToSpec(string? words)
        {
            Assert.Throws<ArgumentException>(() => sentenceValidator.isValid(words));
        }

    }

    [TestFixture]
    public class WordReverserTests
    {
        ISentenceValidator sentenceValidator;
        IWordReverser _reverser;
        const int SENTENCE_MAX_LENGTH_LIMIT = 25;
        [SetUp]
        public void Setup()
        {
            sentenceValidator = new SentenceValidator(SENTENCE_MAX_LENGTH_LIMIT);
            _reverser = new WordReverser(sentenceValidator);
        }

        [Test]
        [TestCase(null)] // null
        [TestCase("")] // zero
        [TestCase("word word word word word word word word word word word word word word word word word word word word word word word word word word word")] //over 25
        public void ReverseSentence_LengthCheck_FailsIfExceeds25SpacesAndCharactersCombined(string? words)
        {
            Assert.Throws<ArgumentException>(() => _reverser.ReverseSentence(words));
        }

        [Test]
        public void ReverseSentence_SentenceCheck_FailsIfSentenceContainsSpecialCharsOtherThanASpace()
        {
            string words = "word !";
            //var result = _reverser.ReverseSentence(words);

            Assert.Throws<ArgumentException>(() => _reverser.ReverseSentence(words));
        }

        [Test]
        public void ReverseSentence_SentenceCheck_FailsIfStartsWithASpace()
        {
            string words = " this is a test";
            //var result = _reverser.ReverseSentence(words);

            Assert.Throws<ArgumentException>(() => _reverser.ReverseSentence(words));
        }

        [Test]
        public void ReverseSentence_SentenceCheck_FailsIfThereisOneOrMoreSpacesInARow()
        {
            string words = "word  ";

            Assert.Throws<ArgumentException>(() => _reverser.ReverseSentence(words));

        }
    }
}
 #endregion
    
 