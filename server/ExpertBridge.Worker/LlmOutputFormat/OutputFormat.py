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


class NsfwDetectionResponse(BaseModel):
    """
    Scores for various types of toxic content, each as a float in [0,1].

    Attributes:
        toxicity: Probability the comment is rude, disrespectful, or unreasonable, likely to make people leave a discussion.
        severe_toxicity: Probability the comment is very hateful, aggressive, or otherwise extremely disruptive.
        obscene: Probability the comment contains profanity or other vulgar language.
        threat: Probability the comment expresses an intention to inflict harm or violence.
        insult: Probability the comment is insulting or demeaning toward an individual or group.
        identity_attack: Probability the comment targets someone based on a protected identity (e.g., race, gender).
        sexual_explicit: Probability the comment contains graphic sexual content or references.
    """

    Toxicity: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description=(
            "A score between 0 and 1 for a rude, disrespectful, or unreasonable comment that is "
            "likely to make people leave a hateful discussion."
        ),
    )
    SevereToxicity: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description=(
            "A score between 0 and 1 for a very hateful, aggressive, or disrespectful comment that is "
            "highly likely to make a user leave a discussion or give up "
            "on sharing their perspective."
        ),
    )
    Obscene: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description="A score between 0 and 1 for a use of profanity, curse words, or other vulgar language.",
    )
    Threat: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description="A score between 0 and 1 for expressions of intent to inflict pain, injury, threats, or violence.",
    )
    Insult: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description="A score between 0 and 1 for an insulting, inflammatory, or demeaning language toward "
                    "an individual or group.",
    )
    IdentityAttack: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description="A score between 0 and 1 for a negative or hateful comments targeting someone based "
                    "on their identity.",
    )
    SexualExplicit: float = Field(
        ...,
        ge=0,
        le=1,
        decimal_places=5,
        description="A score between 0 and 1 for a references to sexual acts, body parts, or other graphic "
                    "sexual content.",
    )


class ProcessedSkill(BaseModel):
    """
    Model representing a processed skill with its name and description.

    Attributes:
        Name: The name of the skill that was processed.
        Description: The description of the skill that was processed.
    """
    Name: str = Field(...,
                            min_length=3,
                            max_length=256,
                            description="The name of the skill that was processed.")
    Description: str = Field(...,
                                   min_length=3,
                                   max_length=256,
                                   description="The description of the skill that was processed.")


class ProcessedSkillsResponse(BaseModel):
    """
    Response model for processed skills.

    Attributes:
        skills: List of processed skills with their names and descriptions.
    """
    Skills: List[ProcessedSkill] = Field(...,
                                          min_length=1,
                                          description="The list of processed skills with their names and descriptions.")


def generate_json_schemas():
    # with open("PostCategorizationOutputFormat.json", "w", encoding="utf-8") as f:
    #     json.dump(CategorizationResponse.model_json_schema(),
    #               f, ensure_ascii=False, indent=2)
    #
    # with open("TranslateTagResponseOutputFormat.json", "w", encoding="utf-8") as f:
    #     json.dump(TranslateTagsResponse.model_json_schema(),
    #               f, ensure_ascii=False, indent=2)
    #
    # with open("NSFWDetectionOutputFormat.json", "w", encoding="utf-8") as f:
    #     json.dump(NsfwDetectionResponse.model_json_schema(),
    #               f, ensure_ascii=False, indent=2)
    #
    with open("ProcessedSkillsOutputFormat.json", "w", encoding="utf-8") as f:
        json.dump(ProcessedSkillsResponse.model_json_schema(),
                  f, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    generate_json_schemas()
