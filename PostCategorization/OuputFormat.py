from pydantic import BaseModel, Field
from typing import List, Literal


class Tags(BaseModel):
    tag: str = Field(...,
                     min_length=3,
                     max_length=256,
                     description="The tag name, separated by a dash '-' and must be lowercase.")

    description: str = Field(...,
                             min_length=3,
                             max_length=512,
                             description="The description of the tag, should be concise and informative.")


LANGUAGES = Literal["English", "Arabic"]


class CategorizationResponse(BaseModel):
    language: LANGUAGES = Field(...,
                                min_length=3,
                                max_digits=50,
                                description="The detected language of the post")

    tags: List[Tags] = Field(...,
                             min_items=3,
                             max_items=5,
                             description="The list of tags for the post with their descriptions")
