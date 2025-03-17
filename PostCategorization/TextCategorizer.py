import json
from groq import Groq
from OuputFormat import CategorizationResponse


class TextCategorizer:
    def __init__(self, api_key: str, model: dict[str, str]):
        """Initialize the Categorizer with Groq API key."""
        self.client = Groq(api_key=api_key)
        self.model = model

        # Define constant prompts and rules
        self.SYSTEM_CONTENT: list[str] = [
            "You are an advanced text categorization AI specializing in both English and Arabic posts.",
            "Your task is to analyze a given post associated with a Pydantic scheme, detect its language, and categorize it with relevant tags.",
            "Provide a structured output with at least three and at most five tags, each accompanied by a brief description.",
            "You have to extract JSON details from text according to the Pydantic scheme.",
            "Do not generate any introductory or concluding text.",
            "Tags and descriptions should be in English regardless of the post's language.",
            "Tags should be relevant to the post problem only.",
            "Tags should be unique and not repetitive.",
            "Tags should not contain numbers, spaces, or special characters except '-'.",
            "Tags should not contain the language name.",
        ]

    def categorize(self, post: str):
        """
        Categorize the given post and return the API response.

        Args:
            post (str): The text content to be categorized

        Returns:
            str: The categorization response from the API
        """
        user_content: list[str] = [
            "Categorize the following post based on its content and language.",
            "Detect whether the post is in English or Arabic, then generate a structured response for the post.",
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
        
        if len(user_content) > MAX_TOKENS:
            raise ValueError("Post content is too long.")

        response = self.client.chat.completions.create(
            messages=[
                {"role": "system", "content": "\n".join(self.SYSTEM_CONTENT)},
                {"role": "user", "content": "\n".join(user_content)}
            ],
            model=self.model["name"],
        )

        return response.choices[0].message.content.strip()
