from fastapi import FastAPI
from pydantic import BaseModel
from transformers import pipeline

app = FastAPI()
qa_pipeline = pipeline("question-answering", model="deepset/roberta-base-squad2")

class QARequest(BaseModel):
    question: str
    context: str

@app.post("/ask")
def ask_question(data: QARequest):
    answer = qa_pipeline(question=data.question, context=data.context)
    return {"answer": answer["answer"]}
