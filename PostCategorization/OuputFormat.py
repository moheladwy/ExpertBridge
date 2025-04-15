from pydantic import BaseModel, Field
from typing import List, Literal


class Tags(BaseModel):
    EnglishName: str = Field(...,
                             min_length=3,
                             max_length=256,
                             description="The English tag name, must be separated by space ' ' and must be lowercase.")

    ArabicName: str = Field(...,
                            min_length=3,
                            max_length=256,
                            description="The Arabic tag name, must be separated by space ' ' and must be lowercase.")

    Description: str = Field(...,
                             min_length=3,
                             max_length=1024,
                             description="The description of the tag, should be concise and informative, and it must be in English and lowercase.")


LANGUAGES = Literal["English", "Arabic", "Mixed", "Other"]


class CategorizationResponse(BaseModel):
    language: LANGUAGES = Field(...,
                                min_length=3,
                                max_length=10,
                                description="The detected language of the post")

    tags: List[Tags] = Field(...,
                             min_length=3,
                             max_length=6,
                             description="The list of tags for the post with their descriptions")
