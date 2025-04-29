from pydantic import BaseModel, Field
from typing import List, Optional


class CategorizeRequest(BaseModel):
    title: str = Field(...,
                       min_length=3,
                       description="The title of the post")

    content: str = Field(...,
                         min_length=3,
                         max_length=5000,
                         description="The content of the post")

    tags: Optional[List[str]] = Field(...,
                                      min_length=0,
                                      max_length=10,
                                      description="Optional list of tags")


class TranslateTagsRequest(BaseModel):
    tags: List[str] = Field(...,
                            min_length=1,
                            description="List of tags to translate")
