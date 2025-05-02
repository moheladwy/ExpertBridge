from pydantic import BaseModel, Field
from typing import List, Literal
import json


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


class TranslateTagsResponse(BaseModel):
    tags: List[Tags] = Field(...,
                             min_length=1,
                             description="The list of translated tags with their descriptions")


def generate_json_schemas():
    with open("PostCategorizationOutputFormat.json", "w", encoding="utf-8") as f:
        json.dump(CategorizationResponse.model_json_schema(),
                  f, ensure_ascii=False, indent=2)

    with open("TranslateTagResponseOutputFormat.json", "w", encoding="utf-8") as f:
        json.dump(TranslateTagsResponse.model_json_schema(),
                  f, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    generate_json_schemas()
