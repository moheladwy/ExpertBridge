import json
from groq import Groq
from pydantic import ValidationError
from OutputFormat import CategorizationResponse, TranslateTagsResponse


class TextCategorizer:
    def __init__(self, api_key: str, model: dict[str, str]):
        """Initialize the Categorizer with Groq API key."""
        self.client = Groq(api_key=api_key)
        self.model = model

    def categorize(self, post: str) -> str:
        """
        Categorize the given post and return the API response.

        Args:
            post (str): The text content to be categorized

        Returns:
            str: The categorization response from the API
        """
        # Define constant prompts and rules
        SYSTEM_CONTENT: list[str] = [
            "You are an advanced text categorization AI specializing in both English and Arabic posts.",
            "Your task is to analyze a given post, detect its language (Arabic, English, Mixed, or Other), and categorize it with relevant tags.",
            "For each tag, you must provide both English and Egyption Arabic names, along with a description.",
            "If the post already has tags, you must translate them and generate additional unique tags.",
            "If the post has tags, translate them without changing their meaning or the existing provided tags.",
            "If the post has no tags, generate new tags from scratch.",
            "Provide a structured output with at least three and at most six tags.",
            "You have to extract JSON details from text according to the Pydantic scheme.",
            "Do not generate any introductory or concluding text.",
            "Tags Names should be in English and Egyption Arabic regardless of the post's language.",
            "Tags should be in lowercase, and separated by space ' '.",
            "Tags should be relevant to the post problem only.",
            "Tags should be unique and not repetitive.",
            "Tags should not contain numbers, or special characters.",
            "Tags should not contain the language name.",
        ]
        USER_CONTENT: list[str] = [
            "Categorize the following post based on its content and language.",
            "1. First, detect whether the post is in English, Arabic, Mixed, or Other.",
            "2. If the post has existing tags, translate them and generate additional unique tags.",
            "3. If the post has no tags, generate new tags from scratch.",
            "4. For each tag, provide both English and Egyption Arabic names, along with a description.",
            "5. Tags should be in lowercase, and separated by space ' '.",
            "6. Tags should not contain numbers, or special characters.",
            "7. Tags should be unique and not repetitive.",
            "### Post Title and Content:",
            "```",
            post.strip(),
            "```",
            "## Pydantic Details:",
            "```",
            json.dumps(
                CategorizationResponse.model_json_schema(), ensure_ascii=False, indent=2
            ),
            "```",
            "Return only the raw JSON without any markdown code block formatting."
        ]

        MAX_TOKENS = int(self.model["MTK"])

        if len(USER_CONTENT) > MAX_TOKENS:
            raise ValueError("Post content is too long.")

        response = self.client.chat.completions.create(
            messages=[
                {"role": "system", "content": "\n".join(SYSTEM_CONTENT)},
                {"role": "user", "content": "\n".join(USER_CONTENT)}
            ],
            model=self.model["name"],
        )

        try:
            result = response.choices[0].message.content.strip()
            CategorizationResponse.model_validate_json(result, strict=True)
            return json.loads(result)
        except ValidationError:
            raise ValueError("Invalid response format from the API.")

    def translate_tags(self, tags: list[str]) -> dict:
        """
        Translate the given tags and return their English and Egyption Arabic names with descriptions.

        Args:
            tags (list[str]): List of tags to translate

        Returns:
            dict: Dictionary containing translated tags with descriptions
        """
        system_content: list[str] = [
            "You are a tag translation specialist. Translate tags between English and Egyption Arabic and provide detailed descriptions in English only.",
            "Your task is to translate the given tags and provide their descriptions.",
            "1. Provide both English and Egyption Arabic translations for each tag.",
            "2. Provide a concise description in English.",
            "3. All names should be lowercase and separated by spaces.",
            "4. If a tag is already in English, provide its Egyption Arabic translation and vice versa.",
            "5. If the language is unclear, try to determine the most likely translation."
            "6. Tags should be unique and not repetitive.",
            "7. Tags should not contain numbers, or special characters.",
            "8. Tags should not contain the language name.",
            "9. Do not generate any introductory or concluding text.",
        ]
        user_content: list[str] = [
            "Translate and provide descriptions for the following tags:",
            "```",
            ", ".join(tags),
            "```",
            "For each tag:",
            "1. Provide both English and Egyption Arabic translations",
            "2. Provide a concise description in English",
            "3. All names should be lowercase and separated by spaces",
            "4. If a tag is already in English, provide its Egyption Arabic translation and vice versa",
            "5. If the language is unclear, try to determine the most likely translation",
            "Return only the raw JSON in the format described below without any markdown code block formatting.",
            "```",
            json.dumps(
                TranslateTagsResponse.model_json_schema(), ensure_ascii=False, indent=2
            ),
            "```"
        ]

        response = self.client.chat.completions.create(
            messages=[
                {"role": "system", "content": "\n".join(system_content)},
                {"role": "user", "content": "\n".join(user_content)}
            ],
            model=self.model["name"],
        )

        try:
            result = response.choices[0].message.content.strip()
            parsed_result = json.loads(result)

            # Validate against TranslateTagsResponse model
            validated_result = TranslateTagsResponse.model_validate(
                parsed_result)
            return validated_result.model_dump()

        except json.JSONDecodeError:
            raise ValueError("Invalid JSON response from the API.")
        except ValidationError as ve:
            raise ValueError(f"API response validation failed: {ve}")
